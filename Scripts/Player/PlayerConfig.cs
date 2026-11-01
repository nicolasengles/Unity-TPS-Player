using UnityEngine;

[System.Serializable]
public struct PlayerConfig
{
    [Header("Jumping")]
    public int MaxJumpCount;

    [Header("Shooting")]
    public float TimeToStopHipfiring;

    [Header("Dashing")]
    public float DashTime;
    public float DashSpeed;
    public float DashCooldown;

    [Header("Animation")]
    [Range(0f, 1f)] public float RunAnimationDampTime;
    public float PlayerRigDampFactor;
}
