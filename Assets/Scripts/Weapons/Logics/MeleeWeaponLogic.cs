using UnityEngine;

public class MeleeWeaponLogic : IWeaponLogic
{
    public readonly MeleeWeaponDataSO m_data;
    public bool m_destroyOnEquip { get; private set; } = false;
    private float m_nextAttackTime = 0f;

    public BaseWeaponData GetData()
    {
        return m_data;
    }

    public MeleeWeaponLogic(MeleeWeaponDataSO data)
    {
        // Set the current SO (from MeleeWeaponDataSO)
        m_data = data;
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
        // Check attack rate
        if (Time.time < m_nextAttackTime) return;
        m_nextAttackTime = Time.time + (1f / m_data.AttackRate);

        PerformMeleeAttack(weapon);

        Debug.Log("<color=orange>Melee attack performed!</color>");
    }

    private void PerformMeleeAttack(WeaponManager weaponManager)
    {
        if (weaponManager.m_eyesCamera == null) return;

        // Get camera and weapons layer
        Transform cam = weaponManager.m_eyesCamera.transform;
        int layerMask = ~LayerMask.GetMask("Weapon");   // ~ = Get all masks BUT not weapon

        // Attack diraction
        Vector3 origin = cam.position;
        Vector3 direction = cam.forward;

        // Use SphereCast to attack also near IDamageables
        RaycastHit hit;
        bool hasHit = Physics.SphereCast(origin, m_data.HitboxRadius, direction, out hit, m_data.Range, layerMask);

        // Check for hit
        if (hasHit)
        {
            // Check for IDamageable object
            // if (hit.transform.TryGetComponent<IDamageable>(out var damageable))
            // {
            //     damageable.TakeDamage(m_data.Damage);
            // }

            Debug.Log($"<color=red>Melee hit object: {hit.transform.name} for {m_data.Damage} damage!</color>");

            // Attack VFX (Check the object that was hit)
            var weaponVfx = weaponManager.GetService<WeaponVFX>();
            weaponVfx?.SpawnEffectBySource(hit.transform.gameObject, hit.point);
        }
        else
        {
            Debug.Log("<color=yellow>Melee missed.</color>");
        }
    }

    public void OnReleaseTrigger() { }

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
}
