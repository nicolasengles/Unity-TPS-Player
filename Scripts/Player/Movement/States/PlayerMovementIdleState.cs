using UnityEngine;

public class PlayerMovementIdleState : PlayerState
{
    // External References
    private InputManager m_inputManager;
    private PlayerMovement m_playerMovement;

    public PlayerMovementIdleState(PlayerMovement playerMovement)
    {
        Name = "Idle";
        CanMove = true;
        CanJump = true;
        CanDash = true;
        CanAim = true;
        CanShoot = true;
        CanReload = true;

        m_inputManager = InputManager.Instance;
        m_playerMovement = playerMovement;
    }

    public override void Enter()
    {
        m_playerMovement.SpeedMultipliers.Add(0f);
    }

    public override void Update()
    {
        if (m_inputManager.HasMovementInput)
        {
            StateMachine.ChangeState<PlayerMovementRunState>();
        }
    }

    public override void Exit()
    {
        m_playerMovement.SpeedMultipliers.Remove(0f);
    }
}
