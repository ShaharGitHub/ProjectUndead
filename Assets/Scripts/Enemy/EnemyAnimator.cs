using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : BaseEnemyService
{
    [Header("Shared controller (same for all humanoid enemies):")]
    [SerializeField] private RuntimeAnimatorController m_controller;

    private Animator m_animator;
    private EnemyStates m_lastState = EnemyStates.Idle;


    private void OnDisable()
    {
        if (m_enemyManager != null)
            m_enemyManager.OnStateChanged -= ChangeAnimation;
    }

    public override void Init()
    {
        base.Init();

        m_enemyManager.OnStateChanged += ChangeAnimation;

        EnemyModel model = m_enemyManager.GetService<EnemyModel>();
        if (model == null) return;

        // The model may already exist (EnemyModel initialized first) or not yet - handle both
        if (model.ModelAnimator != null)
            SetupAnimator(model.ModelAnimator);
        else
            model.OnModelReady += SetupAnimator;
    }

    private void SetupAnimator(Animator modelAnimator)
    {
        m_animator = modelAnimator;
        m_animator.runtimeAnimatorController = m_controller;
        m_animator.applyRootMotion = false; // The NavMeshAgent moves the enemy, not the animation

        // Apply the current state (it may have changed before the model was ready)
        ChangeAnimation(m_lastState);
    }

    private void ChangeAnimation(EnemyStates enemyState)
    {
        m_lastState = enemyState;

        if (m_animator == null) return;

        m_animator.SetInteger("DeathType", -1);
        m_animator.SetInteger("State", -1);

        if (enemyState == EnemyStates.Dead)
            m_animator.SetInteger("DeathType", Random.Range(1, 3));
        else
            m_animator.SetInteger("State", (int)enemyState);
    }
}
