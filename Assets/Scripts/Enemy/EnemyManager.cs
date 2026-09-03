using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IDamageable
{
    // All enemy sub services
    private Dictionary<Type, object> m_services = new Dictionary<Type, object>();

    [Header("Data:")]
    [field: SerializeField] public EnemyDataSO EnemyData { get; private set; }

    [Header("Settings:")]
    [SerializeField] private EnemyStates m_enemyState = EnemyStates.Idle;
    [SerializeField] private float m_attackRange = 10f;
    [SerializeField] private float m_runRange = 100f;
    [SerializeField] private float m_walkRange = 200f;

    [Header("Target:")]
    [field: SerializeField] public Transform m_target { get; private set; }
    [field: SerializeField] public float m_targetDistanceSqr { get; private set; }

    // Events
    public event Action<EnemyStates> OnStateChanged;
    public event Action<float> OnTakeDamage;

    [Header("DEBUG:")]
    [SerializeField] private bool m_IsStateByDistance = true;
    [SerializeField] private EnemyStates m_forceEnemyState;


    private void Start()
    {
        RegisterAllServices();
        InitAllServices();

        OnStateChanged?.Invoke(m_enemyState);
    }

    private void Update()
    {
        CheckDistanceFromTarget();

        // DEUBG
        if (!m_IsStateByDistance)
            ChangeState(m_forceEnemyState);
    }

    public void SetFollowTarget(Transform target)
    {
        m_target = target;
    }

    private void CheckDistanceFromTarget()
    {
        if (!m_IsStateByDistance)
            return;

        if (m_target == null)
            return;

        float distance = Vector3.Distance(transform.position, m_target.position);

        EnemyStates newState;
        if (distance < m_attackRange) newState = EnemyStates.Attack;
        else if (distance < m_runRange) newState = EnemyStates.Run;
        else if (distance < m_walkRange) newState = EnemyStates.Walk;
        else newState = EnemyStates.Idle;

        ChangeState(newState);
    }

    public void ChangeState(EnemyStates newState)
    {
        if (newState != m_enemyState)
        {
            m_enemyState = newState;
            OnStateChanged?.Invoke(m_enemyState);
        }
    }

    public void TakeDamage(float damage)
    {
        OnTakeDamage?.Invoke(damage);
    }

    private void RegisterAllServices()
    {
        // Find all components that use sub services
        var components = GetComponentsInChildren<BaseEnemyService>(true);

        // Register all services to dictionary
        foreach (var service in components)
        {
            // Get service type
            var type = service.GetType();

            // Register service only in not exist in dictionary
            if (!m_services.ContainsKey(type))
            {
                m_services.Add(type, service);
            }

            // Register service also by his interface to dictionary
            var interfaces = type.GetInterfaces();
            foreach (var i in interfaces)
            {
                if (!m_services.ContainsKey(i))
                {
                    m_services.Add(i, service);
                }
            }
        }
    }

    private void InitAllServices()
    {
        // Distinct = Initiate all services BUT to initiate same service twice (service added to list as sub services Or Class name)
        foreach (var service in m_services.Values.OfType<BaseEnemyService>().Distinct())
        {
            service.GetEnemyManager(this);
            service.Init();
        }
    }

    // Where T : class = Ensures T is a class and not a ValueType (like int), allowing the function to return null safely
    public T GetService<T>() where T : class
    {
        if (!m_services.ContainsKey(typeof(T)))
        {
            return null;
        }
        return (T)m_services[typeof(T)];
    }
}
