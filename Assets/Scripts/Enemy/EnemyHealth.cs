using UnityEngine;

public class EnemyHealth : BaseEnemyService
{
    private float m_health;


    public override void Init()
    {
        base.Init();

        if (m_enemyManager == null) return;
        m_enemyManager.OnTakeDamage += HandleDamage;

        if (m_enemyManager.EnemyData != null)
            m_health = m_enemyManager.EnemyData.MaxHealth;
    }

    private void HandleDamage(float damage)
    {
        if (m_health <= 0)
            return;

        m_health -= damage;
        Debug.Log($"{m_enemyManager.name} Hurt, Current health: {m_health}");

        if (m_health <= 0)
        {
            // TEMP
            Debug.Log($"{m_enemyManager.name} is Dead");
            Destroy(m_enemyManager.gameObject);
        }
    }
}
