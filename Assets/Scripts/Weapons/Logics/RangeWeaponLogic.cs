using System.Collections;
using UnityEngine;

public class RangeWeaponLogic : IWeaponLogic, IAmmoWeapon
{
    public readonly RangeWeaponDataSO m_data;
    public bool m_destroyOnEquip { get; private set; } = false;
    private float m_fireRateTimer = 0;

    // Ammo
    public int CurrentClipAmmo { get; private set; }
    public int CurrentReserveAmmo { get; private set; }
    public bool IsReloading { get; private set; }
    private float m_reloadStartTime;


    public BaseWeaponData GetData()
    {
        return m_data;
    }

    public RangeWeaponLogic(RangeWeaponDataSO data)
    {
        // Set the current SO (from RangeWeaponDataSO)
        m_data = data;

        // Ammo
        CurrentReserveAmmo = data.MaxAmmo - data.ClipSize;
        CurrentClipAmmo = data.ClipSize;
    }

    #region IWeaponLogic
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

        // Disable weapon from being a "Shop" (duplication)
        newWeaponManager.DisableDestroyOnEquip();

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
        // ========================== Fire Rate ========================== //

        // Check if enough time has passed since the last shot
        if (Time.time < m_fireRateTimer) return;

        // Calculate the required delay based on the fire rate
        float fireInterval = 1f / m_data.FireRate;

        // Update the timestamp for the next allowed shot
        m_fireRateTimer = Time.time + fireInterval;

        // ========================== Ammo ========================== //

        if (!TryConsumeAmmo())
        {
            TryReload(weaponManager);
            Debug.Log("Clip is empty!");
            return;
        }

        // ========================== Fire ========================== //

        // Send ray cast from weapon muzzle
        MuzzlePosition weaponMuzzle = weaponManager.GetComponentInChildren<MuzzlePosition>();
        if (weaponMuzzle == null) return;

        // Get eyes camera
        if (weaponManager.m_eyesCamera == null) return;
        Transform cam = weaponManager.m_eyesCamera.transform;

        // ~ = Get all masks BUT not weapon
        int layerMask = ~LayerMask.GetMask("Weapon");

        // Check aim point from eyes camera forward
        Vector3 aimPoint;
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit camHit, m_data.Range, layerMask))
        {
            // Eyes hit an object in front of them
            aimPoint = camHit.point;
        }
        else
        {
            // No obhect in front of eye camera
            aimPoint = cam.position + cam.forward * m_data.Range;
        }

        // Shoot diraction = from weapon muzzle to aim point of the eyes camera
        Vector3 rayDirection = (aimPoint - weaponMuzzle.transform.position).normalized;

        // Shoot raycast
        bool didHit = Physics.Raycast(weaponMuzzle.transform.position, rayDirection, out RaycastHit hit, m_data.Range, layerMask);

        // ========================== Effects & Destroy ========================== //

        // Show shoot effect
        var weaponVfx = weaponManager.GetService<WeaponVFX>();
        weaponVfx?.SpawnEffect(weaponMuzzle.gameObject, weaponMuzzle.transform.position, weaponMuzzle.transform.rotation, weaponMuzzle.transform);

        // Check for hits
        if (didHit)
        {
            weaponVfx?.SpawnEffect(hit.transform.gameObject, hit.point);
            Debug.Log($"Weapon hit {hit.transform.name} - {LayerMask.LayerToName(hit.transform.gameObject.layer)}");
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
    #endregion

    #region IAmmoWeapon
    public bool TryConsumeAmmo()
    {
        if (IsReloading)
            return false;

        // No ammo on current clip
        if (CurrentClipAmmo <= 0)
            return false;

        // Decrease current clip ammo
        CurrentClipAmmo--;
        return true;
    }

    public bool TryReload(WeaponManager weaponManager)
    {
        // Can't request new reload when already reload
        if (IsReloading)
            return false;

        // No reserve ammo OR clip is full
        if (CurrentClipAmmo >= m_data.ClipSize || CurrentReserveAmmo <= 0)
            return false;

        // Start reload routine
        weaponManager.StartCoroutine(ReloadRoutine());
        return true;
    }

    private IEnumerator ReloadRoutine()
    {
        IsReloading = true;
        Debug.Log($"Reload started, will take {m_data.ReloadTime}s");

        yield return new WaitForSeconds(m_data.ReloadTime);

        int ammoToLoad = Mathf.Min(m_data.ClipSize - CurrentClipAmmo, CurrentReserveAmmo);
        CurrentClipAmmo += ammoToLoad;
        CurrentReserveAmmo -= ammoToLoad;
        IsReloading = false;

        Debug.Log("Reload finished");
    }
    #endregion
}
