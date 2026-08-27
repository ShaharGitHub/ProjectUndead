using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour, IDamageable
{
    // Enemy data
    [field: SerializeField] public EnemyDataSO EnemyData { get; private set; }

    // All enemy sub services
    private Dictionary<Type, object> m_services = new Dictionary<Type, object>();

    [field: SerializeField] public Transform m_target { get; private set; }

    public event Action<float> OnTakeDamage;


    private void Start()
    {
        RegisterAllServices();
        InitAllServices();
    }

    public void SetFollowTarget(Transform target)
    {
        m_target = target;
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
