using Unity.Hierarchy;
using UnityEngine;

[System.Serializable]
public struct PlayerMovementConfig
{
    [Header("Walk")]
    public float Speed;

    //[Header("Sprint")]
    //public float SprintSpeedMultiplier;

    [Header("Rotation")]
    public float RotationDampSpeed;

    [Header("Aim")]
    [Range(0f, 1f)] public float AimSpeedMultiplier;

    [Header("Physics")]
    public float HorizontalMovementAcceleration;
    public float GravitationalAcceleration;
    public float GroundingForce;
    public float JumpForce;
}