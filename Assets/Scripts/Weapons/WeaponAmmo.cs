using UnityEngine;

public class WeaponAmmo : BaseWeaponService
{
    [SerializeField] private int debug_currentClipAmmo;
    [SerializeField] private int debug_currentReserveAmmo;
    [SerializeField] private bool debug_isReloading;


    // DEBUG
    private void Update()
    {
        debug_currentClipAmmo = GetClipAmmo();
        debug_currentReserveAmmo = GetReserveAmmo();
        debug_isReloading = IsReloading();
    }

    private IAmmoWeapon GetAmmoWeapon()
    {
        return m_weaponManager.GetLogic() as IAmmoWeapon;
    }

    public int GetClipAmmo()
    {
        return GetAmmoWeapon()?.CurrentClipAmmo ?? 0;
    }

    public int GetReserveAmmo()
    {
        return GetAmmoWeapon()?.CurrentReserveAmmo ?? 0;
    }

    public bool IsReloading()
    {
        return GetAmmoWeapon()?.IsReloading ?? false;
    }
}
