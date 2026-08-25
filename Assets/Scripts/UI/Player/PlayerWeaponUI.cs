using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponUI : BasePlayerService
{
    [Header("References:")]
    [SerializeField] private Image m_weaponIcon;
    [SerializeField] private TextMeshProUGUI m_ammoText;

    private WeaponManager m_currentWeapon;


    private void OnDisable()
    {
        m_playerManager.OnWeaponSwitched -= HandleWeaponSwitched;
    }

    public override void Init()
    {
        base.Init();

        if (m_playerManager == null) return;

        m_playerManager.OnWeaponSwitched += HandleWeaponSwitched;
    }

    private void HandleWeaponSwitched(WeaponManager weapon)
    {
        m_currentWeapon = weapon;
    }

    private void Update()
    {
        UpdateWeaponUI();
    }

    private void UpdateWeaponUI()
    {
        if (m_ammoText == null || m_weaponIcon == null)
            return;

        // No weapon in hand
        if (m_currentWeapon == null)
        {
            m_weaponIcon.sprite = null;
            m_ammoText.text = "";
            return;
        }

        // Try get current weapon logic
        IWeaponLogic logic = m_currentWeapon.GetLogic();
        if (logic == null)
            return;

        // Get ammo text
        string ammoText = ""; // Else = Melee weapon
        if (logic is IAmmoWeapon ammoWeapon)
        {
            // Weapon with ammo
            ammoText = $"{ammoWeapon.CurrentClipAmmo}/{ammoWeapon.CurrentReserveAmmo}";
        }
        m_ammoText.text = ammoText;

        // Get weapon icon
        Sprite currentIcon = logic.GetData().Icon;
        m_weaponIcon.sprite = currentIcon;
    }
}
