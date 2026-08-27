using UnityEngine;

public class BaseEnemyService : MonoBehaviour
{
    [SerializeField] protected EnemyManager m_enemyManager;

    public virtual void GetEnemyManager(EnemyManager enemyManager)
    {
        m_enemyManager = enemyManager;
    }

    public virtual void Init() { }
}
