using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player : MonoBehaviour
{
    // Config
    public PlayerConfig Config;

    // Input Manager
    private InputManager m_inputManager;

    // State Machines
    private StateMachine<PlayerState> m_movementSM;
    private StateMachine<PlayerState> m_combatSM;

    // Properties
    [HideInInspector] public bool IsGrounded => m_characterController.isGrounded;
    [HideInInspector] public bool RotateWithCamera { get; set; }
    [HideInInspector] public bool CanMove { get; set; }
    [HideInInspector] public bool CanMoveOnAir { get; set; }
    [HideInInspector] public bool CanJump { get; set; }
    [HideInInspector] public int JumpCount { get; set; }
    [HideInInspector] public bool CanDash { get; set; }
    [HideInInspector] public bool CanAim { get; set; }
    [HideInInspector] public bool CanShoot { get; set; }
    [HideInInspector] public bool CanReload { get; set; }
    [HideInInspector] public Vector3 HorizontalMovementDirection { get; private set; }
    [HideInInspector] public Vector3 FinalVelocity { get; set; }
    [HideInInspector] public float DashCooldown { get; set; }
    [HideInInspector] public float TargetPlayerRigWeight { get; set; }
    [HideInInspector] public Weapon CurrentWeapon { get; private set; }
    [HideInInspector] public bool IsReloading { get; set; }

    // Public Fields
    [HideInInspector] public Vector3 CameraForward;
    [HideInInspector] public Vector3 CameraRight;

    // Serialized Private Fields
    [SerializeField] private Weapon[] m_weapons;

    // Private Fields
    private CharacterController m_characterController;
    private PlayerMovement m_playerMovement;
    private PlayerCamera m_playerCamera;
    private GameObject m_cameraTarget;
    private PlayerCombat m_playerCombat;
    private Animator m_animator;
    private AudioSource m_audioSource;
    private Rig m_playerRig;
    private Coroutine m_hipfireCoroutine;

    // Cache
    private Vector3 m_cachedHorizontalDirection;

    // Debug
    [SerializeField] private GameObject m_debugTextGameObject;
    private TextMeshProUGUI m_debugText;

    private void Awake()
    {
        // Initialize Private Fields
        m_inputManager = InputManager.Instance;
        m_characterController = GetComponent<CharacterController>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerCamera = GetComponent<PlayerCamera>();
        m_cameraTarget = GameObject.Find("CameraTarget");
        m_playerCombat = GetComponent<PlayerCombat>();
        m_animator = GetComponentInChildren<Animator>();
        m_audioSource = GetComponent<AudioSource>();
        m_playerRig = GetComponentInChildren<Rig>();

        // Initialize State Machines
        m_movementSM = new StateMachine<PlayerState>(
            new PlayerState[] 
            {
                new PlayerMovementIdleState(m_playerMovement),
                new PlayerMovementRunState(m_playerMovement, m_animator),
                new PlayerMovementJumpState(this, m_playerMovement),
                new PlayerMovementFallState(this),
                new PlayerMovementDashState(this, m_animator)
            },
            typeof(PlayerMovementIdleState),
            null);

        m_combatSM = new StateMachine<PlayerState>(
            new PlayerState[]
            {
                new PlayerCombatIdleState(),
                new PlayerCombatAimState(this, m_playerMovement, m_playerCamera, m_animator),
                new PlayerCombatHipfireState(this, m_playerCombat, m_animator),
                new PlayerCombatReloadState(this)
            },
            typeof(PlayerCombatIdleState),
            null);

        // Initialize Weapons
        foreach (Weapon weapon in m_weapons)
        {
            weapon.Init();
        }
        CurrentWeapon = m_weapons[0];
    }

    private void OnEnable()
    {
        // Event Subscriptions
        m_inputManager.JumpEvent += JumpEventListener;

        m_inputManager.DashEvent += DashEventListener;

        m_playerMovement.FallStartEvent += FallStartEventListener;
        m_playerMovement.FallEndEvent += FallEndEventListener;

        m_inputManager.ShootStartEvent += HasStartedShootingEventListener;
        m_inputManager.ShootEndEvent += HasStoppedShootingEventListener;

        m_inputManager.AimStartEvent += AimStartEventListener;
        m_inputManager.AimEndEvent += AimEndEventListener;

        m_playerCombat.ShootEvent += ShootEventListener;

        m_inputManager.ReloadEvent += ReloadEventListener;

        m_inputManager.WeaponWheelOpenEvent += WeaponWheelOpenEventListener;
    }

    private void OnDisable()
    {
        // Event Unsubscriptions
        m_inputManager.JumpEvent -= JumpEventListener;
        m_inputManager.DashEvent -= DashEventListener;

        m_playerMovement.FallStartEvent -= FallStartEventListener;
        m_playerMovement.FallEndEvent -= FallEndEventListener;

        m_inputManager.ShootStartEvent -= HasStartedShootingEventListener;
        m_inputManager.ShootEndEvent -= HasStoppedShootingEventListener;

        m_inputManager.AimStartEvent -= AimStartEventListener;
        m_inputManager.AimEndEvent -= AimEndEventListener;

        m_playerCombat.ShootEvent -= ShootEventListener;

        m_inputManager.ReloadEvent -= ReloadEventListener;

        m_inputManager.WeaponWheelOpenEvent -= WeaponWheelOpenEventListener;
    }

    private void Start()
    {
        // Initialize Public Fields
        RotateWithCamera = false;
        CanMoveOnAir = false;
        JumpCount = 0;

        // Initialize Cache
        m_cachedHorizontalDirection = Vector3.zero;

        SetStateConstraints();

        // Debug
        m_debugText = m_debugTextGameObject.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        // Calculate Dash Cooldown
        if (DashCooldown > 0)
            DashCooldown -= Time.deltaTime;

        SetStateConstraints();

        // State Machines Update Calls
        m_movementSM.Update();
        m_combatSM.Update();

        // Move Player
        UpdateHorizontalMovementDirection();
        m_characterController.Move(FinalVelocity);

        // Set Animator Parameters
        m_animator.SetFloat("runAnimationSpeedMultiplier", m_playerMovement.HorizontalVelocity.magnitude / m_playerMovement.Config.Speed);

        Vector3 localMovement = transform.InverseTransformDirection(HorizontalMovementDirection);
        if (RotateWithCamera)
        {
            m_animator.SetFloat("movementX", localMovement.x, Config.RunAnimationDampTime, Time.deltaTime);
            m_animator.SetFloat("movementZ", localMovement.z, Config.RunAnimationDampTime, Time.deltaTime);
        }
        else
        {
            m_animator.SetFloat("movementX", 0);
            m_animator.SetFloat("movementZ", 1);
        }

        m_animator.SetBool("isAirborne", !IsGrounded);

        // Select Active Animation Layers
        if (CurrentWeapon == m_weapons[1])
            m_animator.SetLayerWeight(1, 1);
        else
            m_animator.SetLayerWeight(1, 0);

        m_playerRig.weight = Mathf.Lerp(m_playerRig.weight, RotateWithCamera ? 1 : 0, 1f - Mathf.Exp(-Config.PlayerRigDampFactor * Time.deltaTime));

        SyncAnimation();

        // Debug
        m_debugText.text = $"Movement: {m_movementSM.CurrentState.Name}\nCombat: {m_combatSM.CurrentState.Name}";
    }

    private void UpdateHorizontalMovementDirection()
    {
        // Get Rotation Direction
        CameraForward = m_cameraTarget.transform.forward;
        CameraRight = m_cameraTarget.transform.right;

        // Flatten out Y values
        CameraForward.y = 0f;
        CameraRight.y = 0f;
        CameraForward.Normalize();
        CameraRight.Normalize();

        // Define Horizontal Direction
        if (!IsGrounded && !CanMoveOnAir)
            HorizontalMovementDirection = m_cachedHorizontalDirection;
        else
            HorizontalMovementDirection = ((CameraRight * m_inputManager.MovementInput.x) + (CameraForward * m_inputManager.MovementInput.y)).normalized;

        // Cache
        m_cachedHorizontalDirection = HorizontalMovementDirection;
    }

    private void SetStateConstraints()
    {
        CanMove = m_movementSM.CurrentState.CanMove && m_combatSM.CurrentState.CanMove;
        CanJump = m_movementSM.CurrentState.CanJump && m_combatSM.CurrentState.CanJump;
        CanDash = m_movementSM.CurrentState.CanDash && m_combatSM.CurrentState.CanDash;
        CanAim = m_movementSM.CurrentState.CanAim && m_combatSM.CurrentState.CanAim;
        CanShoot = m_movementSM.CurrentState.CanShoot && m_combatSM.CurrentState.CanShoot;
        CanReload = m_movementSM.CurrentState.CanReload && m_combatSM.CurrentState.CanReload;

        // Check if Unarmed
        if (CurrentWeapon == m_weapons[0])
        {
            CanAim = false;
            CanShoot = false;
        }

        // Check if Empty Ammo
        if (CurrentWeapon.CurrentAmmo <= 0)
            CanShoot = false;

        if (CurrentWeapon.TotalAmmo <= 0)
            CanReload = false;
    }

    public void SetIdealMovementState()
    {
        if (m_inputManager.HasMovementInput)
        {
            m_movementSM.ChangeState<PlayerMovementRunState>();
            return;
        }

        m_movementSM.ChangeState<PlayerMovementIdleState>();
    }

    private void SyncAnimation()
    {
        AnimatorStateInfo currentState = m_animator.GetCurrentAnimatorStateInfo(0);

        if (currentState.IsName("Running"))
        {
            m_animator.SetFloat("runningTime", currentState.normalizedTime % 1f);
        }
        else if (m_animator.IsInTransition(0))
        {
            AnimatorStateInfo nextState = m_animator.GetNextAnimatorStateInfo(0);
            if (nextState.IsName("Running"))
                m_animator.SetFloat("runningTime", nextState.normalizedTime % 1f);
        }
    }

    private void JumpEventListener(object sender, EventArgs e)
    {
        if (CanJump && JumpCount < Config.MaxJumpCount)
            m_movementSM.ChangeState<PlayerMovementJumpState>();

        m_animator.SetTrigger("jump");
    }

    private void DashEventListener(object sender, EventArgs e)
    {
        if (DashCooldown <= 0)
            m_movementSM.ChangeState<PlayerMovementDashState>();
    }

    private void FallStartEventListener(object sender, EventArgs e)
    {
        if (m_movementSM.CurrentState.GetType() != typeof(PlayerMovementDashState))
            m_movementSM.ChangeState<PlayerMovementFallState>();
    }

    private void FallEndEventListener(object sender, EventArgs e)
    {
        m_movementSM.ChangeState<PlayerMovementIdleState>();
        JumpCount = 0;
        m_animator.SetTrigger("fall");
    }

    private void HasStartedShootingEventListener(object sender, EventArgs e)
    {
        if (CanShoot)
        {
            if (m_hipfireCoroutine != null)
            {
                StopCoroutine(m_hipfireCoroutine);
                m_hipfireCoroutine = null;
            }

            if (m_combatSM.CurrentState.GetType() != typeof(PlayerCombatAimState))
                m_combatSM.ChangeState<PlayerCombatHipfireState>();

            m_animator.SetTrigger("shoot");
        }
    }

    private void HasStoppedShootingEventListener(object sender, EventArgs e)
    {
        m_hipfireCoroutine = StartCoroutine(HipfireDuration(Config.TimeToStopHipfiring));
    }

    private IEnumerator HipfireDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (m_combatSM.CurrentState.GetType() == typeof(PlayerCombatHipfireState))
            m_combatSM.ChangeState<PlayerCombatIdleState>();

        m_hipfireCoroutine = null;
    }

    private void AimStartEventListener(object sender, EventArgs e)
    {
        if (CanAim)
            m_combatSM.ChangeState<PlayerCombatAimState>();
    }

    private void AimEndEventListener(object sender, EventArgs e)
    {
        m_combatSM.ChangeState<PlayerCombatIdleState>();
    }

    private void ShootEventListener(object sender, EventArgs e)
    {
        // Animation
        m_animator.SetTrigger("shoot");

        // Add Recoil
        m_playerCamera.TargetPitchRecoil -= UnityEngine.Random.Range(CurrentWeapon.MinPitchRecoil, CurrentWeapon.MaxPitchRecoil);
        m_playerCamera.TargetYawRecoil += UnityEngine.Random.Range(CurrentWeapon.MinYawRecoil, CurrentWeapon.MaxYawRecoil) * (UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1);

        // Play Sound
        m_audioSource.PlayOneShot(CurrentWeapon.FireSound);
    }

    private void ReloadEventListener(object sender, EventArgs e)
    {
        if (CanReload)
            m_combatSM.ChangeState<PlayerCombatReloadState>();
    }

    private void WeaponWheelOpenEventListener(object sender, EventArgs e)
    {
        CurrentWeapon = CurrentWeapon == m_weapons[0] ? m_weapons[1] : m_weapons[0];
        m_playerCombat.SwitchWeaponModel();
    }
}
