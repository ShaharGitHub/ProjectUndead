using UnityEngine;

public class WeaponPickup : BaseWeaponService
{
    private void PickupWeapon()
    {
        IWeapon weaponLogic = m_weaponManager.m_currentWeaponData.CreateWeapon();

        // Connect weapon to player combat script
        //playerCombat.SetWeapon(weaponLogic);
    }
}
