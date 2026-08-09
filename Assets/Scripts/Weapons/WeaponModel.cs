using UnityEngine;

public class WeaponModel : BaseWeaponService
{
    public override void Init()
    {
        base.Init();

        SpawnModel();
    }

    private void SpawnModel()
    {
        BaseWeaponData weaponData = m_weaponManager.m_currentWeaponData;

        if (weaponData == null || weaponData.Prefab == null)
        {
            Debug.LogError($"{m_weaponManager.gameObject.name}({GetType().Name}): Weapon data not found!");
            return;
        }

        Instantiate(weaponData.Prefab, transform.position, Quaternion.identity, transform);
    }
}
