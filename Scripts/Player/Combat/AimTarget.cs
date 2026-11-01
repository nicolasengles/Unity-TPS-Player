using UnityEngine;

[DefaultExecutionOrder(100)]
public class AimTarget : MonoBehaviour
{
    [SerializeField] private float m_distance;
    [SerializeField] private GameObject m_cameraTarget;

    private void Update()
    {
        transform.position = m_cameraTarget.transform.position + m_cameraTarget.transform.forward * m_distance;
    }
}
