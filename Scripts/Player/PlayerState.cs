public class PlayerState : BaseState<PlayerState>
{
    public virtual bool CanMove { get; set; }
    public virtual bool CanJump { get; set; }
    public virtual bool CanDash { get; set; }
    public virtual bool CanAim { get; set; }
    public virtual bool CanShoot { get; set; }
    public virtual bool CanReload { get; set; }
}
