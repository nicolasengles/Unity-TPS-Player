using UnityEngine;

[System.Serializable]
public struct PlayerCameraConfig
{
    [Header("Position")]
    public float CameraClampAngle;
    public Vector3 CameraTargetOffset;

    [Header("Distance from Player")]
    public float CameraDistanceFromPlayer;
    public float CameraDistanceFromPlayerWhenAiming;
    public float CameraDistanceChangeFactor;
}
