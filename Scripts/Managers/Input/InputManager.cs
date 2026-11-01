using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    // Instance
    public static InputManager Instance {  get; private set; }

    // Config
    [SerializeField] private InputManagerConfig m_config;

    // Player Settings
    [SerializeField] private InputSettings m_settings;

    // Input Action Asset
    [SerializeField] private InputActionAsset m_inputActionAsset;

    // Input Actions
    private InputActionMap m_playerInputActionMap;
    private InputActionMap m_menuInputActionMap;
    private InputAction m_horizontalMovementInputAction;
    private InputAction m_lookInputAction;
    private InputAction m_jumpInputAction;
    private InputAction m_aimInputAction;
    private InputAction m_shootInputAction;
    private InputAction m_reloadInputAction;
    private InputAction m_dashInputAction;
    private InputAction m_weaponWheelInputAction;
    private InputAction m_pauseInputAction;
    private InputAction m_unpauseInputAction;

    // Events
    public event EventHandler JumpEvent;
    public event EventHandler DashEvent;
    public event EventHandler AimStartEvent;
    public event EventHandler AimEndEvent;
    public event EventHandler ShootStartEvent;
    public event EventHandler ShootEndEvent;
    public event EventHandler ReloadEvent;
    public event EventHandler WeaponWheelOpenEvent;
    public event EventHandler WeaponWheelCloseEvent;
    public event EventHandler TogglePauseEvent;

    // Properties
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public float LookSensitivity { get; private set; }
    public bool HasMovementInput => MovementInput.sqrMagnitude > m_config.MovementThreshold;

    // Private Fields
    private float m_sensitivityMultiplier = 0.1f;
    private bool m_isAiming;
    private bool m_isShooting;
    private bool m_isWeaponWheelOpen;

    // Cache
    private bool m_cachedAimInput = false;

    void Awake()
    {
        // Instantiate Instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Define Action Maps
        m_playerInputActionMap = m_inputActionAsset.FindActionMap("Player");
        m_menuInputActionMap = m_inputActionAsset.FindActionMap("Menu");
        m_horizontalMovementInputAction = m_playerInputActionMap.FindAction("Move");
        m_lookInputAction = m_playerInputActionMap.FindAction("Look");
        m_aimInputAction = m_playerInputActionMap.FindAction("Aim");
        m_shootInputAction = m_playerInputActionMap.FindAction("Shoot");
        m_reloadInputAction = m_playerInputActionMap.FindAction("Reload");
        m_dashInputAction = m_playerInputActionMap.FindAction("Dash");
        m_weaponWheelInputAction = m_playerInputActionMap.FindAction("WeaponWheel");
        m_jumpInputAction = m_playerInputActionMap.FindAction("Jump");
        m_pauseInputAction = m_playerInputActionMap.FindAction("Pause");
        m_unpauseInputAction = m_menuInputActionMap.FindAction("Return");
    }
    void Start()
    {
        // Initialize Private Fields
        m_isAiming = false;
        m_isShooting = false;
    }

    void Update()
    {
        // Define Current Input Device
        InputDevice m_currentInputDevice = m_lookInputAction.activeControl?.device;

        // Define Movement and Look Input
        MovementInput = m_horizontalMovementInputAction.ReadValue<Vector2>();
        LookInput = m_lookInputAction.ReadValue<Vector2>();

        // Define Look Sensitivity
        LookSensitivity = m_currentInputDevice is Gamepad ? m_settings.GamepadSensitivity : m_settings.MouseSensitivity;
        LookSensitivity *= m_sensitivityMultiplier;

        // Check for Jumping
        if (m_jumpInputAction.WasPressedThisFrame())
            JumpEvent?.Invoke(this, EventArgs.Empty);

        // Check for Aiming
        if (m_aimInputAction.WasPressedThisFrame())
            m_cachedAimInput = m_cachedAimInput == true ? false : true;

        if (m_settings.ToggleAim)
        {
            if (m_aimInputAction.WasPressedThisFrame())
            {
                if (m_isAiming)
                {
                    AimStartEvent?.Invoke(this, EventArgs.Empty);
                    m_isAiming = false;
                }
                else
                {
                    AimEndEvent?.Invoke(this, EventArgs.Empty);
                    m_isAiming = true;
                }
            }
        }
        else
        {
            if (m_aimInputAction.WasPressedThisFrame())
                AimStartEvent?.Invoke(this, EventArgs.Empty);
            if (m_aimInputAction.WasReleasedThisFrame())
                AimEndEvent?.Invoke(this, EventArgs.Empty);
        }

        // Check for Shooting
        if (!m_isShooting && m_shootInputAction.IsPressed())
        {
            m_isShooting = true;
            ShootStartEvent?.Invoke(this, EventArgs.Empty);
        }

        if (m_isShooting && !m_shootInputAction.IsPressed())
        {
            m_isShooting = false;
            ShootEndEvent?.Invoke(this, EventArgs.Empty);
        }

        // Check for Reloading
        if (m_reloadInputAction.WasPressedThisFrame())
            ReloadEvent?.Invoke(this, EventArgs.Empty);

        // Check for Dashing
        if (m_dashInputAction.WasPressedThisFrame())
            DashEvent?.Invoke(this, EventArgs.Empty);

        // Check for Weapon Wheel
        if (!m_isWeaponWheelOpen && m_weaponWheelInputAction.IsPressed())
        {
            m_isWeaponWheelOpen = true;
            WeaponWheelOpenEvent?.Invoke(this, EventArgs.Empty);
        }

        if (m_isWeaponWheelOpen && !m_weaponWheelInputAction.IsPressed())
        {
            m_isWeaponWheelOpen = false;
            WeaponWheelCloseEvent?.Invoke(this, EventArgs.Empty);
        }

        // Check for Toggling Pause
        if (m_pauseInputAction.WasPressedThisFrame() || m_unpauseInputAction.WasPressedThisFrame())
            TogglePauseEvent?.Invoke(this, EventArgs.Empty);
    }

    private void OnEnable()
    {
        // Initialize Input Action Maps
        m_playerInputActionMap.Enable();
        m_menuInputActionMap.Disable();
    }

    public void SwitchActionMap()
    {
        if (m_playerInputActionMap.enabled)
        {
            m_playerInputActionMap.Disable();
            m_menuInputActionMap.Enable();
        }
        else
        {
            m_playerInputActionMap.Enable();
            m_menuInputActionMap.Disable();
        }
    }
}
