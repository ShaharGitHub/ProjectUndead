using System;
using UnityEngine;

public class EnemyModel : BaseEnemyService
{
    public Animator ModelAnimator { get; private set; }
    public event Action<Animator> OnModelReady;

    public override void Init()
    {
        base.Init();

        SpawnModel();
    }

    private void SpawnModel()
    {
        if (m_enemyManager == null || m_enemyManager.EnemyData.Model == null) return;

        GameObject currentModel = Instantiate(m_enemyManager.EnemyData.Model, transform.position, Quaternion.identity, transform);

        ModelAnimator = currentModel.GetComponentInChildren<Animator>();
        if (ModelAnimator == null)
        {
            Debug.LogError($"{currentModel.name}: model has no Animator! Check Rig -> Animation Type");
            return;
        }

        OnModelReady?.Invoke(ModelAnimator);
    }
}
