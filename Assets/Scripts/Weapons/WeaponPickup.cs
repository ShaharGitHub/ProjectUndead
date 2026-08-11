using Unity.VisualScripting;
using UnityEngine;

public class WeaponPickup : BaseWeaponService
{
    [SerializeField] private bool m_destroyOnPickup = false;

    public void PickupWeapon(PlayerCombat playerCombat)
    {
        //BaseWeaponData data = m_weaponManager.GetLogic().GetData();
        //if (data == null)
        //{
        //    Debug.LogError($"{m_weaponManager.gameObject.name}({GetType().Name}): Weapon data not found!");
        //    return;
        //}

        //IWeapon weaponLogic = data.CreateWeapon();
        //weaponLogic.Equip();

        //playerCombat.SetWeapon(weaponLogic);

        // Check if player have already weapon
        Transform playerWeaponPos = playerCombat.transform.GetChild(0);
        if (playerWeaponPos.childCount != 0)
        {
            // Disconnect weapon from player
            // Drop();
            
        }

        // Create new weapon on player hands
        GameObject weapon = Instantiate(m_weaponManager.gameObject, playerWeaponPos);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;

        // Destroy weapon from wall (if true)
        if (m_destroyOnPickup)
            Destroy(m_weaponManager.gameObject);
    }
}
