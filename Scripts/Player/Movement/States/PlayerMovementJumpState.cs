using System;

public class PlayerMovementJumpState : PlayerState
{
    // External References
    private InputManager m_inputManager;
    private Player m_player;
    private PlayerMovement m_playerMovement;

    public PlayerMovementJumpState(Player player, PlayerMovement playerMovement)
    {
        Name = "Jumping";
        CanMove = true;
        CanJump = true;
        CanDash = true;
        CanAim = false;
        CanShoot = false;
        CanReload = false;

        m_inputManager = InputManager.Instance;
        m_player = player;
        m_playerMovement = playerMovement;
    }

    public override void Enter()
    {
        Jump();

        // Event Subscriptions
        m_inputManager.JumpEvent += JumpEventListener;
    }

    public override void Update()
    {
        return;
    }

    public override void Exit()
    {
        // Event Unsubscriptions
        m_inputManager.JumpEvent -= JumpEventListener;
    }

    void JumpEventListener(object sender, EventArgs e)
    {
        if (m_player.CanJump && m_player.JumpCount < m_player.Config.MaxJumpCount)
            Jump();
    }

    void Jump()
    {
        m_playerMovement.JumpForce = m_playerMovement.Config.JumpForce;
        m_player.JumpCount++;
    }
}
