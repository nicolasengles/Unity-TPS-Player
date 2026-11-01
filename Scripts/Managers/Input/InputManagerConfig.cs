using UnityEngine;

[System.Serializable]
public struct InputManagerConfig
{
    [Range(0, 0.1f)] public float MovementThreshold;
}
