using System;
using UnityEngine;

public class WeaponModel : BaseWeaponService
{
    private GameObject m_currentModel;
    public event Action OnModelReady;


    public override void Init()
    {
        SpawnModel();
    }

    public void SpawnModel()
    {
        // Check for weapon data
        BaseWeaponData weaponData = m_weaponManager.m_currentWeaponData;
        if (weaponData == null || weaponData.Prefab == null)
        {
            Debug.LogError($"{m_weaponManager.gameObject.name}({GetType().Name}): Weapon data not found!");
            return;
        }

        // Destroy previous model
        if (m_currentModel != null)
            Destroy(m_currentModel);

        // Create model based on manager data
        m_currentModel = Instantiate(weaponData.Prefab, transform);
        m_currentModel.transform.localPosition = Vector3.zero;
        m_currentModel.transform.localRotation = Quaternion.identity;

        // Fit the collider to current mesh
        if (TryGetComponent<FitCollider>(out FitCollider fitCollider))
            fitCollider.Init();

        // Sent signal that model is ready
        OnModelReady?.Invoke();
    }
}
