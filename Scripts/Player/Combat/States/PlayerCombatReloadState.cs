using UnityEngine;

public class PlayerCombatReloadState : PlayerState
{
    // External References
    private readonly Player m_player;

    // Private Variables
    private float m_timer;

    public PlayerCombatReloadState(Player player)
    {
        Name = "Reloading";
        CanMove = true;
        CanJump = false;
        CanDash = false;
        CanAim = false;
        CanShoot = false;
        CanReload = true;

        m_player = player;
    }

    public override void Enter()
    {
        if (m_player.CurrentWeapon.TotalAmmo <= 0)
        {
            StateMachine.ChangeState<PlayerCombatIdleState>();
            return;
        }

        m_player.IsReloading = true;

        // Timer
        m_timer = m_player.CurrentWeapon.ReloadTime;
    }

    public override void Update()
    {
        m_timer -= Time.deltaTime;

        if (m_timer <= 0 || !m_player.CanReload)
            StateMachine.ChangeState<PlayerCombatIdleState>();
    }

    public override void Exit()
    {
        if (m_timer <= 0)
        {
            int newMagazine = m_player.CurrentWeapon.TotalAmmo < m_player.CurrentWeapon.MagazineCapacity ? m_player.CurrentWeapon.TotalAmmo : m_player.CurrentWeapon.MagazineCapacity;
            m_player.CurrentWeapon.TotalAmmo -= newMagazine - m_player.CurrentWeapon.CurrentAmmo;
            m_player.CurrentWeapon.CurrentAmmo = newMagazine;
        }

        m_player.IsReloading = false;
    }
}
