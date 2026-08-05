using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Player data
    public PlayerDataSO PlayerData {  get; private set; }

    // All player sub services
    private Dictionary<Type, object> m_services = new Dictionary<Type, object>();

    // Input provider
    private IInputProvider m_inputProvider;

    // Events
    public event Action<InputData> OnPlayerInputUpdated;


    private void Start()
    {
        RegisterAllServices();
        InitAllServices();

        m_inputProvider = GetService<IInputProvider>();
    }

    private void OnEnable()
    {
        if (m_inputProvider != null) m_inputProvider.OnInputUpdated += HandleInputUpdated;
    }

    private void OnDisable()
    {
        if (m_inputProvider != null) m_inputProvider.OnInputUpdated -= HandleInputUpdated;
    }

    private void RegisterAllServices()
    {
        // Find all components that use sub services
        var components = GetComponentsInChildren<BasePlayerService>(true);

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
        foreach (var service in m_services.Values.OfType<BasePlayerService>().Distinct())
        {
            service.GetPlayerManager(this);
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

    private void HandleInputUpdated(InputData data)
    {
        OnPlayerInputUpdated?.Invoke(data);
    }
}
