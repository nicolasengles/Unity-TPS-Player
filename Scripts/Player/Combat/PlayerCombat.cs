using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // Config
    public PlayerCombatConfig Config;

    //External References Declaration
    private InputManager m_inputManager;
    private Player m_player;
    [SerializeField] private GameObject m_weaponSocket;
    [SerializeField] private CinemachineCamera m_playerCmCamera;
    [SerializeField] private GameObject m_bulletHolePrefab;

    // Private Variables
    private GameObject WeaponModel;
    private bool m_isShooting;
    private bool m_isWeaponOnCooldown;
    private bool m_bufferedSingleShot;

    // Public Properties
    [HideInInspector] public float SpreadMultiplier;

    // Events
    public event EventHandler ShootEvent;

    private void Awake()
    {
        // External References Definition
        m_inputManager = InputManager.Instance;
        m_player = GetComponent<Player>();
    }

    private void Start()
    {
        m_isShooting = false;
        m_isWeaponOnCooldown = false;
        m_bufferedSingleShot = false;
        SwitchWeaponModel();
    }

    private void OnEnable()
    {
        m_inputManager.ShootStartEvent += HasStartedShootingEventListener;
        m_inputManager.ShootEndEvent += HasStoppedShootingEventListener;
    }

    private void OnDisable()
    {
        m_inputManager.ShootStartEvent -= HasStartedShootingEventListener;
        m_inputManager.ShootEndEvent -= HasStoppedShootingEventListener;
    }

    private void Update()
    {
        if (m_isWeaponOnCooldown)
            return;

        if (m_player.CanShoot && m_player.CurrentWeapon.CurrentAmmo > 0 && ((m_player.CurrentWeapon.IsAuto && m_isShooting) || m_bufferedSingleShot))
        {
            Shoot();
            StartCoroutine(WeaponCooldown(m_player.CurrentWeapon.FireCooldown));
        }

        m_bufferedSingleShot = false;
    }

    private void HasStartedShootingEventListener(object sender, EventArgs e)
    {
        if (m_player.CurrentWeapon.IsAuto)
            m_isShooting = true;
        else
            m_bufferedSingleShot = true;
    }

    private void HasStoppedShootingEventListener(object sender, EventArgs e)
    {
        m_isShooting = false;
    }

    private void Shoot()
    {
        // Trigger Event
        ShootEvent?.Invoke(this, EventArgs.Empty);

        // Calculate Spread
        Vector3 spread = new Vector3(
            UnityEngine.Random.Range(-m_player.CurrentWeapon.MaxSpread, m_player.CurrentWeapon.MaxSpread),
            UnityEngine.Random.Range(-m_player.CurrentWeapon.MaxSpread, m_player.CurrentWeapon.MaxSpread),
            UnityEngine.Random.Range(-m_player.CurrentWeapon.MaxSpread, m_player.CurrentWeapon.MaxSpread)
        );

        // Cast Shot
        Ray ray = new Ray(m_playerCmCamera.transform.position, m_playerCmCamera.transform.forward + spread * SpreadMultiplier);
        if (Physics.Raycast(ray, out RaycastHit hit, m_player.CurrentWeapon.Range))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green, 10f);

            Damageable damageable = hit.collider.gameObject.GetComponent<Damageable>();
            if (damageable != null) 
            {
                damageable.TakeDamage(m_player.CurrentWeapon.Damage);
            }
            else
                Instantiate(m_bulletHolePrefab, hit.point + hit.normal * 0.02f, Quaternion.LookRotation(-hit.normal));

        }
        else
            Debug.DrawLine(ray.origin, ray.direction * 1000f, Color.red, 10f);

        // Reduce ammo in Magazine
        m_player.CurrentWeapon.CurrentAmmo--;
    }

    private IEnumerator WeaponCooldown(float duration)
    {
        m_isWeaponOnCooldown = true;
        yield return new WaitForSeconds(duration);
        m_isWeaponOnCooldown = false;
    }

    public void SwitchWeaponModel()
    {
        if (WeaponModel != null)
            Destroy(WeaponModel);

        if (m_player.CurrentWeapon.Model != null)
        {
            WeaponModel = Instantiate(m_player.CurrentWeapon.Model, m_weaponSocket.transform);
            WeaponModel.transform.localPosition = m_player.CurrentWeapon.LocalPosition;
            WeaponModel.transform.localRotation = m_player.CurrentWeapon.LocalRotation;
            WeaponModel.transform.localScale = Vector3.one;
        }
    }
}
