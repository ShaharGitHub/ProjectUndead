using UnityEngine;

public class RangeWeaponLogic : IWeaponLogic
{
    public readonly RangeWeaponDataSO _data;
    public bool m_destroyOnEquip { get; private set; } = false;


    public BaseWeaponData GetData()
    {
        return _data;
    }

    public RangeWeaponLogic(RangeWeaponDataSO data)
    {
        // Set the current SO (from RangeWeaponDataSO)
        _data = data;
    }

    public void SetDestroyOnEquip(bool stat)
    {
        m_destroyOnEquip = stat;
    }

    public WeaponManager Equip(WeaponManager equipWeapon, Transform handSlot) // Used by "PlayerInteract"
    {
        // Drop previous weapon from player hand
        //if (handSlot.childCount != 0)
        //{
        //    if (handSlot.GetChild(0).TryGetComponent<WeaponManager>(out WeaponManager weaponManager))
        //        Drop(weaponManager);
        //}

        // Create new weapon on player hand
        WeaponManager newWeapon = Object.Instantiate(equipWeapon, handSlot);
        newWeapon.SetData(equipWeapon.GetLogic().GetData());

        // Create new weapon on player hands
        //newWeapon.transform.SetParent(handSlot);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.transform.localScale = Vector3.one;

        // Destroy weapon in m_destroyOnEquip = true
        if (m_destroyOnEquip)
            equipWeapon.DestroyWeapon();

        Debug.Log("Range weapon picked");

        return newWeapon;
    }

    public void Use(WeaponManager weapon) // Used by "PlayerCombat"
    {
        weapon.UseWeapon();

        Debug.Log("Range weapon fire");
    }

    public void Drop(WeaponManager weapon) // Used by "__"
    {
        // Disconnect weapon from player
        weapon.transform.parent = null;

        // Drop weapon from hands forward
        if (weapon.TryGetComponent<Rigidbody>(out Rigidbody weaponRb))
        {
            weaponRb.isKinematic = false;
            weaponRb.AddForce(weapon.transform.forward * 200 + weapon.transform.up * 100);
        }

        Debug.Log("Range Weapon droped");
    }
}
