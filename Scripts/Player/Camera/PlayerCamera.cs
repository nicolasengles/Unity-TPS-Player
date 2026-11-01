using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //Config
    public PlayerCameraConfig Config;

    //Public Fields
    [HideInInspector] public float TargetCameraDistance { get; set; }
    [HideInInspector] public float TargetPitchRecoil { get; set; }
    [HideInInspector] public float TargetYawRecoil { get; set; }

    // Serialized Private Fields
    [SerializeField] private GameObject m_cameraTarget;
    [SerializeField] private CinemachineCamera m_cmPlayerCamera;

    // Private Fields
    private InputManager m_inputManager;
    private Player m_player;
    private CinemachineThirdPersonFollow m_cmPlayerCamera3rdPersonComponent;
    private float m_cameraPitch;
    private float m_cameraYaw;
    private float m_pitchRecoil;
    private float m_yawRecoil;

    void Awake()
    {
        // Initialize Private Fields
        m_inputManager = InputManager.Instance;
        m_player = GetComponent<Player>();
        m_cmPlayerCamera3rdPersonComponent = m_cmPlayerCamera.GetComponent<CinemachineThirdPersonFollow>();
    }

    void Start()
    {
        // Camera Distance Setup
        m_cmPlayerCamera3rdPersonComponent.CameraDistance = Config.CameraDistanceFromPlayer;
        TargetCameraDistance = m_cmPlayerCamera3rdPersonComponent.CameraDistance;
    }

    void Update()
    {
        // Camera Distance Logic
        m_cmPlayerCamera3rdPersonComponent.CameraDistance = Mathf.Lerp(m_cmPlayerCamera3rdPersonComponent.CameraDistance, TargetCameraDistance, Config.CameraDistanceChangeFactor);
    }

    private void LateUpdate()
    {
        // Calculate and Clamp camera pitch
        m_cameraPitch -= m_inputManager.LookInput.y * m_inputManager.LookSensitivity;
        m_cameraPitch = Mathf.Clamp(m_cameraPitch, -Config.CameraClampAngle, Config.CameraClampAngle);

        // Calculate camera yaw
        m_cameraYaw += m_inputManager.LookInput.x * m_inputManager.LookSensitivity;
        m_cameraYaw %= 360f;

        // Calculate Recoil
        m_pitchRecoil = Mathf.Lerp(m_pitchRecoil, TargetPitchRecoil, 1f - Mathf.Exp(-m_player.CurrentWeapon.RecoilKickSpeed * Time.deltaTime));
        m_yawRecoil = Mathf.Lerp(m_yawRecoil, TargetYawRecoil, 1f - Mathf.Exp(-m_player.CurrentWeapon.RecoilKickSpeed * Time.deltaTime));

        // Recover from Recoil
        TargetPitchRecoil = Mathf.Lerp(TargetPitchRecoil, 0f, 1f - Mathf.Exp(-m_player.CurrentWeapon.RecoilRecoverySpeed * Time.deltaTime));
        TargetYawRecoil = Mathf.Lerp(TargetYawRecoil, 0f, 1f - Mathf.Exp(-m_player.CurrentWeapon.RecoilRecoverySpeed * Time.deltaTime));

        // Rotate the camera
        m_cameraTarget.transform.position = transform.position + Config.CameraTargetOffset;
        m_cameraTarget.transform.rotation = Quaternion.Euler(m_cameraPitch + m_pitchRecoil, m_cameraYaw + m_yawRecoil, 0f);
    }
}
