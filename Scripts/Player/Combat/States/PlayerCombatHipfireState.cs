using UnityEngine;

public class PlayerCombatHipfireState : PlayerState
{
    // External References
    private readonly Player m_player;
    private readonly PlayerCombat m_playerCombat;
    private readonly Animator m_animator;

    public PlayerCombatHipfireState(Player player, PlayerCombat playerCombat, Animator animator)
    {
        Name = "Hipfiring";
        CanMove = true;
        CanJump = true;
        CanDash = true;
        CanAim = true;
        CanShoot = true;
        CanReload = true;

        m_player = player;
        m_playerCombat = playerCombat;
        m_animator = animator;
    }

    public override void Enter()
    {
        // Camera
        m_player.RotateWithCamera = true;

        // Combat
        m_playerCombat.SpreadMultiplier = m_playerCombat.Config.HipfireSpreadMultiplier;

        // Animation
        m_animator.SetBool("isHipfiring", true);
    }

    public override void Update()
    {
        if (!m_player.CanShoot)
            StateMachine.ChangeState<PlayerCombatIdleState>();
    }

    public override void Exit()
    {
        // Camera
        m_player.RotateWithCamera = false;

        // Combat
        m_playerCombat.SpreadMultiplier = 0f;

        // Animation
        m_animator.SetBool("isHipfiring", false);
    }
}
