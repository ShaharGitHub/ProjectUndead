using UnityEngine;
using UnityEngine.AI;

public class EnemyControl : BaseEnemyService
{
    private NavMeshAgent m_agent;


    private void OnDisable()
    {
        m_enemyManager.OnStateChanged -= SetAgentStats;
    }

    public override void Init()
    {
        base.Init();

        if (m_enemyManager != null)
            m_agent = m_enemyManager.GetComponent<NavMeshAgent>();

        m_enemyManager.OnStateChanged += SetAgentStats;

        //SetAgentStats();
    }

    private void Update()
    {
        if (m_agent == null || m_enemyManager == null || m_enemyManager.m_target == null)
            return;

        m_agent.SetDestination(m_enemyManager.m_target.position);

        // DEBUG
        //SetAgentStats();
    }

    private void SetAgentStats(EnemyStates enemyState)
    {
        if (m_agent == null || m_enemyManager == null)
            return;

        switch (enemyState)
        {
            default:
            case EnemyStates.Idle:
                m_agent.speed = 0;
                break;

            case EnemyStates.Walk:
                m_agent.speed = m_enemyManager.EnemyData.WalkSpeed;
                break;

            case EnemyStates.Run:
                m_agent.speed = m_enemyManager.EnemyData.SprintSpeed;
                break;

            case EnemyStates.Attack:
                m_agent.speed = 0;
                break;

            case EnemyStates.Dead:
                m_agent.speed = 0;
                break;
        }
    }
}
