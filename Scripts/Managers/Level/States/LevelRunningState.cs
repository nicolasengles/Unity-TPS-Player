using System;
using UnityEngine;

public class LevelRunningState : LevelState
{
    // Input Manager
    private InputManager m_inputManager = InputManager.Instance;

    // Private Fields
    private Canvas m_HUDCanvas;

    public LevelRunningState(Canvas hudCanvas)
    {
        Name = "Running";
        m_HUDCanvas = hudCanvas;
    }

    public override void Enter()
    {
        m_HUDCanvas.enabled = true;

        // Event Subscriptions
        m_inputManager.TogglePauseEvent += TogglePauseEventListener;
    }

    public override void Exit()
    {
        m_HUDCanvas.enabled = false;

        // Event Unsubscriptions
        m_inputManager.TogglePauseEvent -= TogglePauseEventListener;
    }

    private void TogglePauseEventListener(object sender, EventArgs e)
    {
        StateMachine.ChangeState<LevelPausedState>();
    }
}
