using UnityEngine;

public class DetachFromParentOnAwake : MonoBehaviour
{
    void Awake()
    {
        transform.SetParent(null);
    }
}
