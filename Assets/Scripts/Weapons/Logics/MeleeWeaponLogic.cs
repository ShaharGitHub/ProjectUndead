using UnityEngine;

public class MeleeWeaponLogic : IWeaponLogic
{
    public readonly MeleeWeaponDataSO _data;
    public bool m_destroyOnEquip { get; private set; } = false;

    public BaseWeaponData GetData()
    {
        return _data;
    }

    public MeleeWeaponLogic(MeleeWeaponDataSO data)
    {
        // Set the current SO (from MeleeWeaponDataSO)
        _data = data;
    }

    public void SetDestroyOnEquip(bool stat)
    {
        m_destroyOnEquip = stat;
    }

    public WeaponManager Equip(WeaponManager equipWeapon, Transform handSlot) // Used by "PlayerInteract"
    {
        //// Drop previous weapon from player hand
        //if (previousWeapon != null)
        //    Drop(previousWeapon);

        // Create new weapon on player hand
        WeaponManager newWeapon = equipWeapon;

        // Destroy weapon in m_destroyOnEquip = true
        if (m_destroyOnEquip)
            equipWeapon.DestroyWeapon();

        Debug.Log("Melee weapon equip");

        return null;
    }

    public void Use(WeaponManager weapon) // Used by "PlayerCombat"
    {
        //weapon.UseWeapon();

        Debug.Log("Melee weapon swish");
    }

    public void Drop(WeaponManager weapon) // Used by "__"
    {
        // Disconnect weapon from player
        weapon.transform.parent = null;

        // Drop weapon from hands forward
        if (weapon.TryGetComponent<Rigidbody>(out Rigidbody weaponRb))
        {
            weaponRb.AddForce(weapon.transform.right * -200 + weapon.transform.up * 100);
        }

        Debug.Log("Melee Weapon droped");
    }
}
