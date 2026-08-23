using UnityEngine;

public class ThrowableWeaponLogic : IWeaponLogic
{
    public readonly ThrowableWeaponDataSO _data;
    public bool m_destroyOnEquip { get; private set; } = false;

    public BaseWeaponData GetData()
    {
        return _data;
    }

    public ThrowableWeaponLogic(ThrowableWeaponDataSO data)
    {
        // Set the current SO (from ThrowableWeaponDataSO)
        _data = data;
    }

    public void SetDestroyOnEquip(bool stat)
    {
        m_destroyOnEquip = stat;
    }

    public WeaponManager Equip(WeaponManager equipWeapon, GameObject weaponPrefab, Transform handSlot, Camera eyesCamera) // Used by "PlayerInteract"
    {
        //// Drop previous weapon from player hand
        //if (previousWeapon != null)
        //    Drop(previousWeapon);

        // Create new weapon on player hand
        WeaponManager newWeapon = equipWeapon;

        // Destroy weapon in m_destroyOnEquip = true
        if (m_destroyOnEquip)
            equipWeapon.DestroyWeapon();

        Debug.Log("Throwable weapon equiped");

        return null;
    }

    public void Use(WeaponManager weapon) // Used by "PlayerCombat"
    {
        //weapon.UseWeapon();

        Debug.Log("Throwable weapon used");
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

        Debug.Log("Throwable Weapon droped");
    }
}
