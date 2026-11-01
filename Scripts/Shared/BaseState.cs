public abstract class BaseState<TState> where TState : BaseState<TState>
{
    public virtual StateMachine<TState> StateMachine { get; set; }
    public virtual string Name { get; set; }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
