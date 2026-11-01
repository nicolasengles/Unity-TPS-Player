using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // State Machines
    private StateMachine<LevelState> m_levelStateMachine;

    // Serialized Private Fields
    [SerializeField] private Canvas m_HUDCanvas;
    [SerializeField] private Canvas m_pauseMenuCanvas;

    void Start()
    {
        m_pauseMenuCanvas.enabled = false;

        // Initialize State Machines
        m_levelStateMachine = new StateMachine<LevelState>(
            new LevelState[]
            {
                new LevelRunningState(m_HUDCanvas),
                new LevelPausedState(m_pauseMenuCanvas)
            },
            typeof(LevelRunningState),
            null);
    }

    void Update()
    {
        // Update State Machines
        m_levelStateMachine.Update();
    }
}
