public class PlayerMovementFallState : PlayerState
{
    // External References
    private Player m_player;

    public PlayerMovementFallState(Player player)
    {
        Name = "Falling";
        CanMove = true;
        CanJump = true;
        CanDash = true;
        CanAim = false;
        CanShoot = false;
        CanReload = false;

        m_player = player;
    }

    public override void Enter() { return; }

    public override void Update() { return; }

    public override void Exit() { return; }
}
