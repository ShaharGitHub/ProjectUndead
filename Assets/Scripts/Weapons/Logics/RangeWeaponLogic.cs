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

    public WeaponManager Equip(WeaponManager pickedWeapon, GameObject weaponPrefab, Transform handSlot, Camera eyesCamera)
    {
        // Create new weapon on player hand
        GameObject newWeaponObj = Object.Instantiate(weaponPrefab, handSlot);
        WeaponManager newWeaponManager = newWeaponObj.GetComponent<WeaponManager>();

        // Reset weapon transform on player hand
        newWeaponManager.transform.localPosition = Vector3.zero;
        newWeaponManager.transform.localRotation = Quaternion.identity;
        newWeaponManager.transform.localScale = Vector3.one;

        // Set data for new weapon
        newWeaponManager.SetData(pickedWeapon.GetLogic().GetData());

        // Get player eyes camera
        if (eyesCamera != null)
            newWeaponManager.SetEyes(eyesCamera);

        // Stop weapon gravity
        if (newWeaponManager.TryGetComponent<Rigidbody>(out Rigidbody weaponRb))
            weaponRb.isKinematic = true;

        // Destroy weapon in m_destroyOnEquip = true
        if (m_destroyOnEquip)
            pickedWeapon.DestroyWeapon();

        Debug.Log("Range weaponManager picked");

        return newWeaponManager;
    }

    public void Use(WeaponManager weaponManager)
    {
        // Send ray cast from weapon muzzle
        MuzzlePosition weaponMuzzle = weaponManager.GetComponentInChildren<MuzzlePosition>();
        if (weaponMuzzle == null) return;

        if (weaponManager.m_eyesCamera == null) return;
        Vector3 rayOrigin = weaponManager.m_eyesCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

        // Check for hits
        if (Physics.Raycast(weaponMuzzle.transform.position, rayOrigin, out RaycastHit hit, m_data.Range))
        {
            Debug.Log($"Range weaponManager fire and hit: {hit.transform.name}");
            Debug.DrawRay(weaponMuzzle.transform.position, hit.point, Color.blue);
        }
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
