using UnityEngine;

public class PlayerModel : BasePlayerService
{
    public override void Init()
    {
        base.Init();

        SpawnModel();
    }

    private void SpawnModel()
    {
        if (m_playerManager == null || m_playerManager.PlayerData.Model == null) return;

        GameObject currentModel = Instantiate(m_playerManager.PlayerData.Model, transform);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;
    }
}
