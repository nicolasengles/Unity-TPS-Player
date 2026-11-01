using System.Security.Claims;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementRunState : PlayerState
{
    // External References
    private PlayerMovement m_playerMovement;
    private Animator m_animator;

    public PlayerMovementRunState(PlayerMovement playerMovement, Animator animator)
    {
        Name = "Running";
        CanMove = true;
        CanJump = true;
        CanDash = true;
        CanAim = true;
        CanShoot = true;
        CanReload = true;

        m_playerMovement = playerMovement;
        m_animator = animator;
    }

    public override void Enter()
    {
        m_animator.SetBool("isRunning", true);
    }

    public override void Update()
    {
        if (m_playerMovement.HorizontalVelocity.sqrMagnitude == 0)
        {
            StateMachine.ChangeState<PlayerMovementIdleState>();
        }
    }

    public override void Exit()
    {
        m_animator.SetBool("isRunning", false);
    }
}
