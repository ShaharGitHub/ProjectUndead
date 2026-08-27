using UnityEngine;

public class EnemyModel : BaseEnemyService
{
    public override void Init()
    {
        base.Init();

        SpawnModel();
    }

    private void SpawnModel()
    {
        if (m_enemyManager == null || m_enemyManager.EnemyData.Model == null) return;

        GameObject currentModel = Instantiate(m_enemyManager.EnemyData.Model, transform.position, Quaternion.identity, transform);
    }
}
