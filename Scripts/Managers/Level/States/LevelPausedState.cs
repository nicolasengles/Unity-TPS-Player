using System;
using UnityEngine;

public class LevelPausedState : LevelState
{
    // Private Fields
    private InputManager m_inputManager;
    private Canvas m_pauseMenuCanvas;

    public LevelPausedState(Canvas pauseMenuCanvas)
    {
        Name = "Paused";
        m_inputManager = InputManager.Instance;
        m_pauseMenuCanvas = pauseMenuCanvas;
    }

    public override void Enter()
    {
        Time.timeScale = 0f;
        m_pauseMenuCanvas.enabled = true;
        m_inputManager.SwitchActionMap();

        // Event Subscriptions
        m_inputManager.TogglePauseEvent += TogglePauseEventListener;
    }

    public override void Exit()
    {
        Time.timeScale = 1.0f;
        m_pauseMenuCanvas.enabled = false;
        m_inputManager.SwitchActionMap();

        // Event Unsubscriptions
        m_inputManager.TogglePauseEvent -= TogglePauseEventListener;
    }

    private void TogglePauseEventListener(object sender, EventArgs e)
    {
        StateMachine.ChangeState<LevelRunningState>();
    }
}
