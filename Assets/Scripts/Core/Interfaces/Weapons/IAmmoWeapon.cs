using UnityEngine;

public interface IAmmoWeapon
{
    int CurrentClipAmmo { get; }    // How much ammo the clip have
    int CurrentReserveAmmo { get; } // How much ammo the weapon have in reserve
    bool IsReloading { get; }

    bool TryConsumeAmmo();          // Try to use ammo
    bool TryReload(WeaponManager weaponManager);
    void TryAddAmmo();
}
