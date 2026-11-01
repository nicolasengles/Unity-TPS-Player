using System;
using System.Collections.Generic;

public class StateMachine<TState> where TState : BaseState<TState>
{
    private readonly Dictionary<Type, TState> m_states = new Dictionary<Type, TState>();
    private System.Action m_runGlobalConditionCheck;

    public TState CurrentState { get; private set; }

    public StateMachine(TState[] states, Type initialState, System.Action globalConditionCheck)
    {
        foreach (TState state in states)
        {
            Type stateType = state.GetType();

            if (m_states.ContainsKey(stateType))
            {
                throw new Exception("Duplicate state: " + stateType.Name);
            }

            m_states.Add(stateType, state);
            state.StateMachine = this;
        }
 
        m_runGlobalConditionCheck = globalConditionCheck;

        TState currentState;
        if (!m_states.TryGetValue(initialState, out currentState))
        {
            throw new Exception("Initial state is not in states list");
        }
        CurrentState = currentState;
        CurrentState.Enter();
    }

    public void ChangeState<T>() where T : TState
    {
        TState newState;

        if (!m_states.TryGetValue(typeof(T), out newState))
        {
            throw new Exception("State does not exist in the current context: ");
        }

        if (CurrentState == newState)
            return;

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState.Update();
        if (m_runGlobalConditionCheck != null)
            m_runGlobalConditionCheck();
    }
}
