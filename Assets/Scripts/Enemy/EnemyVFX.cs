using UnityEngine;
using UnityEngine.AI;

public class EnemyVFX : BaseEnemyService
{
    [SerializeField] private RuntimeAnimatorController m_controller;
    [SerializeField] private Animator m_animator;
    private NavMeshAgent m_agent;

    public float baseAnimationSpeed = 0.25f;


    public override void Init()
    {
        base.Init();
        Invoke(nameof(GetAnimatorDelay), 0.1f);
    }

    private void Update()
    {
        if (m_animator == null || m_agent == null)
            return;


        float currentSpeed = m_agent.velocity.magnitude;

        // מכוון את קצב הפעלת האנימציה כך שיתאים למהירות בפועל
        float speedRatio = currentSpeed / baseAnimationSpeed;
        m_animator.speed = Mathf.Clamp(speedRatio, 0.01f, 3f); // הגנה מפני 0 או קפיצות

        // אופציונלי: פרמטר לבלנד בין idle להליכה
        //m_animator.SetFloat("Speed", currentSpeed);
    }

    private void GetAnimatorDelay()
    {
        if (m_enemyManager != null)
        {
            m_animator = m_enemyManager.GetComponentInChildren<Animator>();

            m_agent = m_enemyManager.GetComponent<NavMeshAgent>();

            //if (m_animator == null || m_controller == null)
            //    return;

            //m_animator.runtimeAnimatorController = m_controller;
            //m_animator.applyRootMotion = false;
        }
    }

    //public float SetDeath()
    //{
    //    int random = Random.Range(1, 3);

    //    m_animator.SetInteger("IsDead", random);

    //    AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
    //    float currentClipLength = stateInfo.length;
    //    return currentClipLength;
    //}
}
