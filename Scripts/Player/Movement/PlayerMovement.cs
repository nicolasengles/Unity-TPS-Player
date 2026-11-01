using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Config
    public PlayerMovementConfig Config;

    // External References
    private Player m_player;

    // Internal Variables
    private Vector3 m_verticalVelocity;

    // Public Properties
    [HideInInspector] public float TargetSpeed { get; private set; }
    [HideInInspector] public List<float> SpeedMultipliers { get; private set; }
    [HideInInspector] public float JumpForce { get; set; }
    [HideInInspector] public Vector3 HorizontalVelocity { get; private set; }
    [HideInInspector] public float SpeedMultiplier { get; private set; }

    // Public Variables
    [HideInInspector] public Vector3 FinalMovementVelocity { get; private set; }

    // Cache
    private bool m_isFalling;
    private bool m_isGrounded;

    // Events
    public event EventHandler FallStartEvent;
    public event EventHandler FallEndEvent;

    void Awake()
    {
        m_player = GetComponent<Player>();
        SpeedMultipliers = new List<float>();
    }

    private void Start()
    {
        JumpForce = 0f;
        m_isFalling = false;
        m_isGrounded = m_player.IsGrounded;
        FinalMovementVelocity = Vector3.zero;
    }

    void Update()
    {
        bool hasMoved = false;
        // Check for fall
        if (!m_player.IsGrounded && m_verticalVelocity.y < 0 && !m_isFalling)
        {
            FallStartEvent?.Invoke(this, EventArgs.Empty);
            m_isFalling = true;
            m_isGrounded = false;
        }
        if (m_player.IsGrounded && !m_isGrounded)
        {
            FallEndEvent?.Invoke(this, EventArgs.Empty);
            m_isGrounded = true;
            m_isFalling = false;
        }

        // Jump
        m_verticalVelocity.y += JumpForce;
        JumpForce = 0f;

        // Add gravity
        if (m_player.IsGrounded && m_verticalVelocity.y < 0)
        {
            m_verticalVelocity.y = -Config.GroundingForce;
        }
        m_verticalVelocity.y -= Config.GravitationalAcceleration * Time.deltaTime;

        if (m_player.CanMove && !hasMoved)
        {
            // Rotate the Character
            if (m_player.RotateWithCamera)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_player.CameraForward), 1f);
            if (m_player.HorizontalMovementDirection != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_player.HorizontalMovementDirection), Time.deltaTime * Config.RotationDampSpeed);

            // Define Speed Multiplier
            SpeedMultiplier = 1f;
            SpeedMultipliers.ForEach(x => SpeedMultiplier *= x);

            // Define Horizontal Velocity
            HorizontalVelocity = Vector3.MoveTowards(HorizontalVelocity, m_player.HorizontalMovementDirection * Config.Speed * SpeedMultiplier, Config.HorizontalMovementAcceleration * Time.deltaTime);

            // Move
            m_player.FinalVelocity = new Vector3(HorizontalVelocity.x, m_verticalVelocity.y, HorizontalVelocity.z) * Time.deltaTime;
        }
        hasMoved = false;
    }
}
