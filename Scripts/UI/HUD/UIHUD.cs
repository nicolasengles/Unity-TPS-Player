using TMPro;
using UnityEngine;

public class UIHUD : MonoBehaviour
{
    // External References Declaration
    private Player m_player;
    private TextMeshProUGUI m_weaponText;
    private TextMeshProUGUI m_ammoText;

    private void Awake()
    {
        m_player = GameObject.Find("Player").GetComponent<Player>();
        m_weaponText = transform.Find("Weapon").GetComponent<TextMeshProUGUI>();
        m_ammoText = transform.Find("Ammo").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    private void Update()
    {
        m_weaponText.text = $"Weapon: {m_player.CurrentWeapon.name}";

        if (m_player.IsReloading)
            m_ammoText.text = "Ammo: Reloading...";
        else
            m_ammoText.text = $"Ammo: {m_player.CurrentWeapon.CurrentAmmo} / {m_player.CurrentWeapon.TotalAmmo}";
    }
}
