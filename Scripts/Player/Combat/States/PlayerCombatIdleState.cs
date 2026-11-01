public class PlayerCombatIdleState : PlayerState
{
    public PlayerCombatIdleState()
    {
        Name = "Idle";
        CanMove = true;
        CanJump = true;
        CanDash = true;
        CanAim = true;
        CanShoot = true;
        CanReload = true;
    }
}
