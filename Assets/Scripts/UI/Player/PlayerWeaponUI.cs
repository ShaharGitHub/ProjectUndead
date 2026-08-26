using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponUI : BasePlayerService
{
    [Header("Settings:")]
    [SerializeField] private float m_reloadIconSpeed;

    [Header("References:")]
    [SerializeField] private Image m_weaponIcon;
    [SerializeField] private Image m_reloadIcon;
    [SerializeField] private TextMeshProUGUI m_ammoText;
    [SerializeField] private Transform m_fireModeParent;

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
        if (m_currentWeapon == null)
        {
            ResetWeaponUI();
            return;
        }

        // Try get current weapon logic
        IWeaponLogic logic = m_currentWeapon.GetLogic();
        if (logic == null)
            return;

        IAmmoWeapon currentAmmoWeapon = null;  // Melee Weapon
        if (logic is IAmmoWeapon ammoWeapon)
            currentAmmoWeapon = ammoWeapon;    // Weapon with ammo

        SetAmmoText(currentAmmoWeapon);
        SetAmmoIcon(logic);
        SetReloadIcon(currentAmmoWeapon);
        SetFireMode(logic);
    }

    private void SetAmmoText(IAmmoWeapon currentAmmoWeapon)
    {
        if (m_ammoText == null)
            return;

        // Get ammo text
        string ammoText = ""; // Else = Melee weapon
        if (currentAmmoWeapon != null)
            ammoText = $"{currentAmmoWeapon.CurrentClipAmmo}/{currentAmmoWeapon.CurrentReserveAmmo}";

        m_ammoText.text = ammoText;
    }

    private void SetAmmoIcon(IWeaponLogic logic)
    {
        if (m_weaponIcon == null)
            return;

        // Get weapon icon
        m_weaponIcon.sprite = logic.GetData().Icon;
    }

    private void SetReloadIcon(IAmmoWeapon currentAmmoWeapon)
    {
        if (m_reloadIcon == null)
            return;

        // Reload icon
        if (currentAmmoWeapon != null)
        {
            // Show/Hide reload icon
            m_reloadIcon.gameObject.SetActive(currentAmmoWeapon.IsReloading);

            if (currentAmmoWeapon.IsReloading)
            {
                // Active animation
                m_reloadIcon.transform.Rotate(0, 0, m_reloadIconSpeed);
            }
            else
            {
                // Disable animation
                m_reloadIcon.transform.rotation = Quaternion.identity;
            }
        }
        else
        {
            m_reloadIcon.gameObject.SetActive(false);
        }
    }

    private void SetFireMode(IWeaponLogic logic)
    {
        if (m_fireModeParent == null)
            return;

        if (logic is RangeWeaponLogic rangeWeapon)
        {
            m_fireModeParent.gameObject.SetActive(true);

            int activeIndex = 0;
            switch (rangeWeapon.m_data.FireMode)
            {
                case FireModes.Semi:
                    activeIndex = 0;
                    break;
                case FireModes.Burst:
                    activeIndex = 1;
                    break;
                case FireModes.Auto:
                    activeIndex = 2;
                    break;
            }

            ModifyFireModeChilds(activeIndex);
        }
        else
        {
            m_fireModeParent.gameObject.SetActive(false);
        }
    }

    private void ModifyFireModeChilds(int activeIndex)
    {
        if (m_fireModeParent == null)
            return;

        for (int i = 0; i < m_fireModeParent.transform.childCount; i++)
        {
            Image img = m_fireModeParent.transform.GetChild(i).GetComponent<Image>();
            if (img == null)
                continue;

            string hexColor = (i <= activeIndex) ? "#FFFFFF" : "#7E7E7E";

            if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
            {
                img.color = newColor;
            }
        }
    }

    private void ResetWeaponUI()
    {
        if (m_ammoText != null)
            m_ammoText.text = "";

        if (m_weaponIcon != null)
            m_weaponIcon.sprite = null;

        if (m_reloadIcon != null)
            m_reloadIcon.gameObject.SetActive(false);

        if (m_fireModeParent != null)
            m_fireModeParent.gameObject.SetActive(false);
    }
}
