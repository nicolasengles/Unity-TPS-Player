using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera m_MainCamera;

    void Awake()
    {
        m_MainCamera = Camera.main;
    }

    void LateUpdate()
    {
        transform.rotation = m_MainCamera.transform.rotation;
    }
}
