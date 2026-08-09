using Unity.VisualScripting;
using UnityEngine;

public class WeaponPickup : BaseWeaponService
{
    [SerializeField] private bool m_destroyOnPickup = false;

    public void PickupWeapon(PlayerCombat playerCombat)
    {
        BaseWeaponData data = m_weaponManager.m_currentWeaponData;
        if (data == null)
        {
            Debug.LogError($"{m_weaponManager.gameObject.name}({GetType().Name}): Weapon data not found!");
            return;
        }

        IWeapon weaponLogic = data.CreateWeapon();
        weaponLogic.Pickup();

        playerCombat.SetWeapon(weaponLogic);

        // Attach weapon to player
        Transform playerWeaponPos = playerCombat.transform.GetChild(0);
        if (playerWeaponPos.childCount != 0)
        {
            if (playerWeaponPos.GetChild(0).transform.TryGetComponent<WeaponManager>(out WeaponManager oldWeapon))
            {
                //oldWeapon.Drop();
                oldWeapon.transform.parent = null;

                Rigidbody weaponRb = oldWeapon.transform.AddComponent<Rigidbody>();
                weaponRb.AddForce(transform.right * -200 + transform.up * 100);
            }
            else
            {
                Destroy(playerWeaponPos.GetChild(0).gameObject);
            }
        }

        GameObject weapon = Instantiate(m_weaponManager.gameObject, playerWeaponPos);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        if (m_destroyOnPickup)
            Destroy(m_weaponManager.gameObject);
    }
}
