using System.Collections;
using UnityEngine;

public class ThrowableWeaponLogic : IWeaponLogic, IAmmoWeapon
{
    public readonly ThrowableWeaponDataSO m_data;
    public bool m_destroyOnEquip { get; private set; } = false;
    private bool m_isHoldingToThrow = false;
    private WeaponManager m_currentWeaponManager;

    // Ammo
    public int CurrentClipAmmo { get; private set; }
    public int CurrentReserveAmmo { get; private set; }
    public bool IsReloading { get; private set; }


    public BaseWeaponData GetData()
    {
        return m_data;
    }

    public ThrowableWeaponLogic(ThrowableWeaponDataSO data)
    {
        // Set the current SO (from ThrowableWeaponDataSO)
        m_data = data;

        // Ammo
        CurrentClipAmmo = m_data.MaxAmmo;
        CurrentReserveAmmo = 0;
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

    public void Use(WeaponManager weapon)
    {
        if (m_isHoldingToThrow)
            return;

        if (!TryConsumeAmmo())
        {
            //Debug.Log("<color=red>No grenades left!</color>");
            return;
        }

        m_isHoldingToThrow = true;

        // Get current weapon
        m_currentWeaponManager = weapon;

        // Start Explosive routine
        weapon.StartCoroutine(ExplodeRoutine(weapon));

        //Debug.Log("<color=orange>Grenade thrown!</color>");
    }

    public void OnReleaseTrigger()
    {
        // No holding a throwable OR No throwable in hand
        if (!m_isHoldingToThrow || m_currentWeaponManager == null)
            return;

        m_isHoldingToThrow = false; // Stop holding

        // Disconnect throwable from hand
        m_currentWeaponManager.transform.parent = null;

        // Active physics
        if (m_currentWeaponManager.TryGetComponent<Rigidbody>(out Rigidbody weaponRb))
        {
            weaponRb.isKinematic = false;
            weaponRb.mass = 1.5f;
            weaponRb.AddForce(m_currentWeaponManager.transform.forward * 700 + m_currentWeaponManager.transform.up * 300);
            //weaponRb.AddTorque(Random.insideUnitSphere * 1f);
        }

        //Debug.Log("<color=cyan>Grenade thrown into the world!</color>");

        // Clear current weapon
        m_currentWeaponManager = null;
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
        // No ammo on current clip
        if (CurrentClipAmmo <= 0)
            return false;

        // Decrease current clip ammo
        CurrentClipAmmo--;
        return true;
    }

    public bool TryReload(WeaponManager weaponManager)
    {
        return false;
    }

    private IEnumerator ExplodeRoutine(WeaponManager weapon)
    {
        // Safe granade can't trigger on hand (Exp: smoke)
        if (m_data.TimeToExpload < 0 && m_isHoldingToThrow)
            yield return new WaitUntil(() => !m_isHoldingToThrow);

        // Granade time to exploade
        yield return new WaitForSeconds(Mathf.Abs(m_data.TimeToExpload));

        // The throwable destroyed for some reason
        if (weapon == null)
            yield break;

        // Explosive position
        Vector3 explosionPos = weapon.transform.position;

        if (m_data.IsDamage)
        {
            // Check all colliders that effected by the radius
            Collider[] colliders = Physics.OverlapSphere(explosionPos, m_data.Radius);

            // Run over each collider
            foreach (var hit in colliders)
            {
                // Don't exploade self
                if (hit.gameObject == weapon.gameObject)
                    continue;

                // Check for IDamageable objects
                if (hit.transform != null && hit.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    damageable.TakeDamage(m_data.Damage);
                }

                Debug.Log($"Explosion hit: <color=cyan>{hit.name}</color>");
            }
        }

        // Create explosive effect
        var weaponVfx = weapon.GetService<WeaponVFX>();
        weaponVfx?.SpawnEffect(m_data.VfxType, weapon.transform.position, Quaternion.Euler(-90, 0, 0));

        // Destroy the granade
        Object.Destroy(weapon.gameObject);
    }

    public void TryAddAmmo()
    {
        if (CurrentReserveAmmo >= m_data.MaxAmmo)
            return;

        CurrentReserveAmmo = Mathf.Min(CurrentReserveAmmo + 1, m_data.MaxAmmo);
    }
    #endregion
}
