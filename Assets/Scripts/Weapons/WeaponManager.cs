using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Data:")]
    private Dictionary<Type, object> m_weaponServices = new Dictionary<Type, object>();
    [field: SerializeField] public BaseWeaponData m_currentWeaponData { get; private set; }


    private void Awake()
    {
        RegisterAllServices();
    }

    public void Start()
    {
        if (m_currentWeaponData != null)
        {
            InitAllServices();
            // Try to add delay to fitcollider (until model is spawn)
            GetService<WeaponModel>().OnModelReady += HandleModelIsReady;
        }
    }

    public void SetData(BaseWeaponData weaponData)
    {
        m_currentWeaponData = weaponData;
        InitAllServices();
    }

    private void RegisterAllServices()
    {
        // Find all components that use BaseWeaponService
        var components = GetComponentsInChildren<BaseWeaponService>(true);

        // Register all services to dictionary
        foreach (var service in components)
        {
            // Get service type
            var type = service.GetType();

            // Register service only in not exist in dictionary
            if (!m_weaponServices.ContainsKey(type))
            {
                m_weaponServices.Add(type, service);
            }

            // Register service also by his interface to dictionary
            var interfaces = type.GetInterfaces();
            foreach (var i in interfaces)
            {
                if (!m_weaponServices.ContainsKey(i))
                {
                    m_weaponServices.Add(i, service);
                }
            }
        }
    }

    private void InitAllServices()
    {
        // Distinct = Initiate all services BUT to initiate same service twice (service added to list as BaseWeaponService Or Class name)
        foreach (var service in m_weaponServices.Values.OfType<BaseWeaponService>().Distinct())
        {
            service.GetWeaponManager(this);
            service.Init();
        }
    }

    // Where T : class = Ensures T is a class and not a ValueType (like int), allowing the function to return null safely
    public T GetService<T>() where T : class
    {
        if (!m_weaponServices.ContainsKey(typeof(T)))
        {
            return null;
        }
        return (T)m_weaponServices[typeof(T)];
    }

    private void HandleModelIsReady()
    {
        if (TryGetComponent<FitCollider>(out FitCollider fitCollider))
            fitCollider.Init();
    }

    private void OnDestroy()
    {
        GetService<WeaponModel>().OnModelReady -= HandleModelIsReady;
    }
}
