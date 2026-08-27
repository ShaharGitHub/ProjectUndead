using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyControl : BaseEnemyService
{
    private NavMeshAgent m_agent;
    private Transform m_target;


    public override void Init()
    {
        base.Init();

        m_agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (m_agent == null || m_target == null)
            return;

        m_agent.SetDestination(m_target.position);
    }

    public void SetAgentTarget(Transform target)
    {
        m_target = target;
    }
}
