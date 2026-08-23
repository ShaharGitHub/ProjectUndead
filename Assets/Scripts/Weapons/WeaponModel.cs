using UnityEngine;

public class WeaponModel : BaseWeaponService
{
    private GameObject m_currentModel;


    public override void Init()
    {
        base.Init();

        SpawnModel();
    }

    public void SpawnModel()
    {
        BaseWeaponData weaponData = m_weaponManager.GetLogic().GetData();

        if (weaponData == null || weaponData.Prefab == null)
        {
            Debug.LogError($"{m_weaponManager.gameObject.name}({GetType().Name}): Weapon data not found!");
            return;
        }

        if (m_currentModel != null)
        {
            Destroy(m_currentModel);
        }

        m_currentModel = Instantiate(weaponData.Prefab, transform);
        m_currentModel.transform.localPosition = Vector3.zero;
        m_currentModel.transform.localRotation = Quaternion.identity;

        if (m_weaponManager.TryGetComponent<FitCollider>(out FitCollider fitCollider))
            fitCollider.Init();
    }
}
