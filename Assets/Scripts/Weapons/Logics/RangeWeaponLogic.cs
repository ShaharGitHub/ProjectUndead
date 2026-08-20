using UnityEngine;

public class RangeWeaponLogic : IWeaponLogic
{
    public readonly RangeWeaponDataSO m_data;
    public bool m_destroyOnEquip { get; private set; } = false;


    public BaseWeaponData GetData()
    {
        return m_data;
    }

    public RangeWeaponLogic(RangeWeaponDataSO data)
    {
        // Set the current SO (from RangeWeaponDataSO)
        m_data = data;
    }

    public void SetDestroyOnEquip(bool stat)
    {
        m_destroyOnEquip = stat;
    }

    public WeaponManager Equip(WeaponManager equipWeapon, Transform handSlot)
    {
        // Create new weapon on player hand
        WeaponManager newWeapon = Object.Instantiate(equipWeapon, handSlot);
        newWeapon.SetData(equipWeapon.GetLogic().GetData());
        newWeapon.DisableDestroyOnEquip();

        // Create new weapon on player hands
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.transform.localScale = Vector3.one;

        // Stop weapon gravity
        if (newWeapon.TryGetComponent<Rigidbody>(out Rigidbody weaponRb))
        {
            weaponRb.isKinematic = true;
        }

        // Destroy weapon in m_destroyOnEquip = true
        if (m_destroyOnEquip)
            equipWeapon.DestroyWeapon();

        Debug.Log("Range weapon picked");

        return newWeapon;
    }

    public void Use(WeaponManager weapon)
    {
        // TEMP - Send ray cast from main camera

        // CANT BE MAIN CAMERA !!
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

        // Check for hits
        if (Physics.Raycast(rayOrigin, Camera.main.transform.forward, out RaycastHit hit, m_data.Range))
        {
            Debug.Log($"Range weapon fire and hit: {hit.transform.name}");
            Debug.DrawRay(rayOrigin, hit.point, Color.blue);
        }

        /*
        //// TEMP - Send ray cast from weapon muzzle
        //Transform weaponMuzzle = weapon.GetComponentInChildren<MuzzlePosition>().transform;
        //if (weaponMuzzle == null) return;

        //Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

        //// Check for hits
        //if (Physics.Raycast(weaponMuzzle.position, rayOrigin, out RaycastHit hit, m_data.Range))
        //{
        //    Debug.Log($"Range weapon fire and hit: {hit.transform.name}");
        //    Debug.DrawRay(weaponMuzzle.position, hit.point, Color.blue);
        //}
        */
    }

    public void Drop(WeaponManager weapon)
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
