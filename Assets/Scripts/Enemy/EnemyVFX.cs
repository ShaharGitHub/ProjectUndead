using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVFX : BaseEnemyService
{
    [SerializeField] private float m_fallbackBaseSpeed = 0.25f;
    private Dictionary<string, float> m_clipBaseSpeeds = new Dictionary<string, float>();
    private Animator m_animator;
    private NavMeshAgent m_agent;


    private void OnDisable()
    {
        m_enemyManager.OnStateChanged -= ChangeAnimation;
    }

    public override void Init()
    {
        base.Init();
        Invoke(nameof(GetReferences), 0.1f);

        m_enemyManager.OnStateChanged += ChangeAnimation;
    }

    private void Update()
    {
        if (m_animator == null || m_agent == null)
            return;

        UpdateAnimator();
    }

    private void GetReferences()
    {
        if (m_enemyManager != null)
        {
            m_animator = m_enemyManager.GetComponentInChildren<Animator>();
            m_agent = m_enemyManager.GetComponent<NavMeshAgent>();

            if (m_animator == null || m_agent == null)
                return;

            // Disable animator root motion
            m_animator.applyRootMotion = false;

            CacheAllClipSpeeds();
        }
    }

    private void CacheAllClipSpeeds()
    {
        // Get all animator clips
        AnimationClip[] clips = m_animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            // Get animation avarage speed
            float speed = clip.averageSpeed.magnitude;

            if (speed > 0.001f)
            {
                // Svae animation avarage speed in dictionary
                m_clipBaseSpeeds[clip.name] = speed;
            }
        }
    }

    private void UpdateAnimator()
    {
        // Get current animation
        AnimatorClipInfo[] clipInfo = m_animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length == 0)
            return;

        // Get current animation name
        string currentClipName = clipInfo[0].clip.name;

        // Get current animation base speed from dictionary
        float baseSpeed = m_clipBaseSpeeds.TryGetValue(currentClipName, out float cachedSpeed)
            ? cachedSpeed
            : m_fallbackBaseSpeed;

        // Get NavMesh speed and calculate animator speed
        float currentSpeed = m_agent.velocity.magnitude;
        float speedRatio = currentSpeed / baseSpeed;

        // Update animator speed
        m_animator.speed = Mathf.Clamp(speedRatio, 0.01f, 3f);
    }

    private void ChangeAnimation(EnemyStates enemyState)
    {
        if (m_animator != null)
            m_animator.SetInteger("State", (int)enemyState);
    }
}
