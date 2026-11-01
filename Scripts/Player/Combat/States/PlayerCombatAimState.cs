using UnityEngine;

public class PlayerCombatAimState : PlayerState
{
    // External References
    private readonly Player m_player;
    private readonly PlayerMovement m_playerMovement;
    private readonly PlayerCamera m_playerCamera;
    private readonly Animator m_animator;

    public PlayerCombatAimState(Player player, PlayerMovement playerMovement, PlayerCamera playerCamera, Animator animator)
    {
        Name = "Aiming";
        CanMove = true;
        CanJump = true;
        CanDash = true;
        CanAim = true;
        CanShoot = true;
        CanReload = true;

        m_player = player;
        m_playerMovement = playerMovement;
        m_playerCamera = playerCamera;
        m_animator = animator;
    }

    public override void Enter()
    {
        // Movement
        m_playerMovement.SpeedMultipliers.Add(m_playerMovement.Config.AimSpeedMultiplier);

        // Camera
        m_player.RotateWithCamera = true;
        m_playerCamera.TargetCameraDistance = m_playerCamera.Config.CameraDistanceFromPlayerWhenAiming;

        // Animation
        m_animator.SetBool("isAiming", true);
        m_player.TargetPlayerRigWeight = 1f;
    }

    public override void Update()
    {
        if (!m_player.CanAim)
            StateMachine.ChangeState<PlayerCombatIdleState>();
    }

    public override void Exit()
    {
        // Movement
        m_playerMovement.SpeedMultipliers.Remove(m_playerMovement.Config.AimSpeedMultiplier);
        //m_player.CanSprint = true;

        // Camera
        m_player.RotateWithCamera = false;
        m_playerCamera.TargetCameraDistance = m_playerCamera.Config.CameraDistanceFromPlayer;

        // Animation
        m_animator.SetBool("isAiming", false);
        m_player.TargetPlayerRigWeight = 0;
    }
}
