using UnityEngine;

public class PlayerMovementDashState : PlayerState
{
    private readonly InputManager m_inputManager;
    private readonly Player m_player;
    private readonly Animator m_animator;
    private float m_timer;
    private Vector3 direction;

    public PlayerMovementDashState(Player player, Animator animator)
    {
        Name = "Dashing";
        CanMove = false;
        CanJump = false;
        CanDash = true;
        CanAim = false;
        CanShoot = false;
        CanReload = false;

        m_inputManager = InputManager.Instance;
        m_player = player;
        m_animator = animator;
    }

    public override void Enter()
    {
        m_animator.SetBool("isDashing", true);
        m_timer = m_player.Config.DashTime;

        if (m_inputManager.HasMovementInput)
            direction = m_player.HorizontalMovementDirection;
        else
            direction = m_player.CameraForward;
    }

    public override void Update()
    {
        m_timer -= Time.deltaTime;
        if (m_timer <= 0 || !m_player.CanDash)
            m_player.SetIdealMovementState();

        m_player.FinalVelocity = direction * m_player.Config.DashSpeed * Time.deltaTime;
    }

    public override void Exit()
    {
        m_animator.SetBool("isDashing", false);
        m_player.DashCooldown = m_player.Config.DashCooldown;
    }
}
