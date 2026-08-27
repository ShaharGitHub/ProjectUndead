using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RangeWeaponLogic : IWeaponLogic, IAmmoWeapon
{
    public readonly RangeWeaponDataSO m_data;
    public bool m_destroyOnEquip { get; private set; } = false;
    private float m_fireRateTimer;
    private bool m_isBurst = false;
    private bool m_wasTriggerReleased = true;

    // Ammo
    public int CurrentClipAmmo { get; private set; }
    public int CurrentReserveAmmo { get; private set; }
    public bool IsReloading { get; private set; }


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
        WeaponManager returnedWeaponManager;

        if (!m_destroyOnEquip) // <= Store
        {
            // Create new weapon on player hand
            GameObject newWeaponObj = Object.Instantiate(weaponPrefab, handSlot);
            returnedWeaponManager = newWeaponObj.GetComponent<WeaponManager>();

            // Set data for new weapon
            returnedWeaponManager.SetData(pickedWeapon.GetLogic().GetData());

            // Disable weapon from being a "Shop" (duplication)
            returnedWeaponManager.DisableDestroyOnEquip();
        }
        else // <= Not store
        {
            returnedWeaponManager = pickedWeapon;
            returnedWeaponManager.transform.SetParent(handSlot);
            returnedWeaponManager.SetSelfDestroy(false); // Cancel self destroy (despawn)
        }

        // Reset weapon transform on player hand
        returnedWeaponManager.transform.localPosition = Vector3.zero;
        returnedWeaponManager.transform.localRotation = Quaternion.identity;
        returnedWeaponManager.transform.localScale = Vector3.one;

        // Get player eyes camera
        if (eyesCamera != null)
            returnedWeaponManager.SetEyes(eyesCamera);

        // Stop weapon gravity
        if (returnedWeaponManager.TryGetComponent<Rigidbody>(out Rigidbody weaponRb))
            weaponRb.isKinematic = true;

        //Debug.Log("Range weaponManager picked");

        return returnedWeaponManager;
    }

    public void Use(WeaponManager weaponManager)
    {
        // ========================== Fire Mode ========================== //

        // Don't stop burst shot
        if (m_isBurst) return;

        // Player not released the trigger with semi weapon
        if (m_data.FireMode == FireModes.Semi && !m_wasTriggerReleased)
            return;

        // Check fire rate
        if (Time.time < m_fireRateTimer) return;

        switch (m_data.FireMode)
        {
            case FireModes.Auto:
                FireBullet(weaponManager);
                m_fireRateTimer = Time.time + (1f / m_data.FireRate); // Update fire rate
                break;

            case FireModes.Semi:
                m_wasTriggerReleased = false; // Disable shot if holding trigger
                FireBullet(weaponManager);
                m_fireRateTimer = Time.time + (1f / m_data.FireRate); // Update fire rate
                break;

            case FireModes.Burst:
                weaponManager.StartCoroutine(FireBurstRoutine(weaponManager));
                break;
        }
    }

    private IEnumerator FireBurstRoutine(WeaponManager weaponManager)
    {
        m_isBurst = true;

        //// Optional: Calculate fire rate for burst
        //float burstInterval = (1f / m_data.FireRate) * 0.75f;

        for (int i = 0; i < 3; i++)
        {
            FireBullet(weaponManager);

            if (CurrentClipAmmo <= 0)
                break;

            yield return new WaitForSeconds(0.1f); // Delay between shoots
        }

        m_fireRateTimer = Time.time + (1f / m_data.FireRate); // Update fire rate
        m_isBurst = false;
    }

    private void FireBullet(WeaponManager weaponManager)
    {
        // ========================== Ammo ========================== //

        if (!TryConsumeAmmo())
        {
            return;
        }

        if (CurrentClipAmmo <= 0)
        {
            TryReload(weaponManager);
            Debug.Log("Clip is empty!");
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

        // Check for IDamageable objects
        if (hit.transform != null && hit.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable?.TakeDamage(m_data.Damage);
        }

        // ========================== Effects & Destroy ========================== //

        // Show shoot effect
        var weaponVfx = weaponManager.GetService<WeaponVFX>();
        weaponVfx?.SpawnEffect(VfxTypes.MuzzleFlash, weaponMuzzle.transform.position, weaponMuzzle.transform.rotation, weaponMuzzle.transform);

        // Check for hits
        if (didHit)
        {
            weaponVfx?.SpawnEffectBySource(hit.transform.gameObject, hit.point);
            //Debug.Log($"Weapon hit {hit.transform.name} - {LayerMask.LayerToName(hit.transform.gameObject.layer)}");
        }

        // ========================== Sounds ========================== //
        // weaponManager.VFX
    }

    public void ChangeFireMode()
    {
        m_data.FireMode = (FireModes)(((int)m_data.FireMode + 1) % System.Enum.GetValues(typeof(FireModes)).Length);
    }

    public void OnReleaseTrigger()
    {
        m_wasTriggerReleased = true;
    }

    public void Drop(WeaponManager weapon)
    {
        // Disconnect weapon from player
        weapon.transform.parent = null;

        // Drop weapon from hands forward
        if (weapon.TryGetComponent<Rigidbody>(out Rigidbody weaponRb))
        {
            weaponRb.isKinematic = false;
            weaponRb.mass = 2f;
            weaponRb.AddForce(weapon.transform.forward * 200 + weapon.transform.up * 100);
            weaponRb.AddTorque(Random.insideUnitSphere * 1f);
        }

        // Active self destroy (despawn)
        weapon.SetSelfDestroy(true);

        //Debug.Log("Range Weapon droped");
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
