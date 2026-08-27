using UnityEngine.AI;

public class EnemyControl : BaseEnemyService
{
    private NavMeshAgent m_agent;


    public override void Init()
    {
        base.Init();

        if (m_enemyManager != null)
            m_agent = m_enemyManager.GetComponent<NavMeshAgent>();

        SetAgentStats();
    }

    private void Update()
    {
        if (m_agent == null || m_enemyManager == null || m_enemyManager.m_target == null)
            return;

        m_agent.SetDestination(m_enemyManager.m_target.position);

        // DEBUG
        SetAgentStats();
    }

    private void SetAgentStats()
    {
        if (m_agent == null || m_enemyManager == null)
            return;

        m_agent.speed = m_enemyManager.EnemyData.WalkSpeed;
    }
}
