using UnityEngine;
using UnityEngine.AI;

public class EnemyVFX : BaseEnemyService
{
    //[SerializeField] private RuntimeAnimatorController m_controller;
    //[SerializeField] private Animator m_animator;
    //private NavMeshAgent m_agent;


    //public override void Init()
    //{
    //    base.Init();
    //    Invoke(nameof(GetAnimatorDelay), 0.1f);
    //}

    //private void Update()
    //{
    //    if (m_animator == null || m_agent == null || m_enemyManager == null)
    //        return;


    //    float currentSpeed = m_agent.velocity.magnitude;

    //    if (m_enemyManager.EnemyData.WalkSpeed > 0)
    //    {
    //        m_animator.speed = currentSpeed / m_enemyManager.EnemyData.WalkSpeed;
    //    }
    //}

    //private void GetAnimatorDelay()
    //{
    //    if (m_enemyManager != null)
    //    {
    //        m_animator = m_enemyManager.GetComponentInChildren<Animator>();

    //        m_agent = m_enemyManager.GetComponent<NavMeshAgent>();
    //        if (m_agent == null)
    //            m_agent = m_enemyManager.GetComponentInChildren<NavMeshAgent>();

    //        if (m_animator == null || m_controller == null)
    //            return;

    //        m_animator.runtimeAnimatorController = m_controller;
    //        m_animator.applyRootMotion = false;
    //    }
    //}

    //public float SetDeath()
    //{
    //    int random = Random.Range(1, 3);

    //    m_animator.SetInteger("IsDead", random);

    //    AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
    //    float currentClipLength = stateInfo.length;
    //    return currentClipLength;
    //}
}
