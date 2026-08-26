using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour, IWeapon, IInteractable
{
    private Dictionary<Type, object> m_weaponServices = new Dictionary<Type, object>();

    [Header("Data:")]
    [SerializeField] private BaseWeaponData m_currentWeaponData;
    [SerializeField] private IWeaponLogic m_currentWeaponLogic;
    [SerializeField] private float m_timeToSelfDestroy;
    private Coroutine m_selfDestroyCoroutine;
    [SerializeField] private bool m_destroyOnEquip = false;

    public Camera m_eyesCamera { get; private set; }


    private void Awake()
    {
        RegisterAllServices();
    }

    public void Start()
    {
        if (m_currentWeaponData != null)
        {
            m_currentWeaponLogic = m_currentWeaponData.CreateWeapon();
            m_currentWeaponLogic.SetDestroyOnEquip(m_destroyOnEquip);

            InitAllServices();
        }
    }

    public void SetEyes(Camera eyesCamera)
    {
        m_eyesCamera = eyesCamera;
    }

    public void SetSelfDestroy(bool toDestroy)
    {
        if (toDestroy)
        {
            if (m_selfDestroyCoroutine != null)
            {
                StopCoroutine(m_selfDestroyCoroutine);
            }
            m_selfDestroyCoroutine = StartCoroutine(SelfDestroyRoutine(m_timeToSelfDestroy));
            Debug.Log($"<color=red>{m_currentWeaponData.Name}</color> <color=red>activate Self Destroy!</color>");
        }
        else
        {
            if (m_selfDestroyCoroutine != null)
            {
                StopCoroutine(m_selfDestroyCoroutine);
                Debug.Log($"{m_currentWeaponData.Name} cancel <color=green>Self Destroy</color>");
            }
        }
    }

    public IEnumerator SelfDestroyRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // ======================================== IInteractable ======================================== //

    public string GetInteractPrompt()
    {
        return GlobalData.Prompts.Interact + m_currentWeaponData.Name;
    }

    public void Interact(PlayerInteract playerInteract)
    {
        playerInteract.InteractWeapon(this);
    }

    // ======================================== IWeapon ======================================== //

    public void SetData(BaseWeaponData weaponData)
    {
        m_currentWeaponData = weaponData;
        m_currentWeaponLogic = m_currentWeaponData.CreateWeapon();
        m_currentWeaponLogic.SetDestroyOnEquip(m_destroyOnEquip);
        InitAllServices();
    }

    public IWeaponLogic GetLogic()
    {
        return m_currentWeaponLogic;
    }

    public void DisableDestroyOnEquip()
    {
        m_destroyOnEquip = true;
    }

    public void DestroyWeapon()
    {
        Destroy(gameObject);
    }

    // ======================================== Services ======================================== //

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
}
