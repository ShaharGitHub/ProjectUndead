using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour, IWeapon, IInteractable, IPriceable
{
    private Dictionary<Type, object> _weaponServices = new Dictionary<Type, object>();

    [Header("Data:")]
    [SerializeField] private BaseWeaponData _currentWeaponData;
    [SerializeField] private IWeaponLogic _currentWeaponLogic;
    [SerializeField] private float _timeToSelfDestroy = 5;
    private Coroutine _selfDestroyCoroutine;
    [SerializeField] private bool _destroyOnEquip = false;
    [SerializeField] private bool _allowInteract = true;

    public event Action OnInteracted;

    public Camera _eyesCamera { get; private set; }


    private void Awake()
    {
        RegisterAllServices();
    }

    public void Start()
    {
        if (_currentWeaponData != null)
        {
            _currentWeaponLogic = _currentWeaponData.CreateWeapon();
            _currentWeaponLogic.SetDestroyOnEquip(_destroyOnEquip);

            InitAllServices();
        }
    }

    public void SetEyes(Camera eyesCamera)
    {
        _eyesCamera = eyesCamera;
    }

    public void SetData(BaseWeaponData weaponData)
    {
        _currentWeaponData = weaponData;
        _currentWeaponLogic = _currentWeaponData.CreateWeapon();
        _currentWeaponLogic.SetDestroyOnEquip(_destroyOnEquip);
        InitAllServices();
    }

    public void SetSelfDestroy(bool toDestroy)
    {
        if (toDestroy)
        {
            if (_selfDestroyCoroutine != null)
            {
                StopCoroutine(_selfDestroyCoroutine);
            }
            _selfDestroyCoroutine = StartCoroutine(SelfDestroyRoutine(_timeToSelfDestroy));
            //Debug.Log($"<color=cyan>{m_currentWeaponData.Name}</color> <color=red>activate Self Destroy!</color>");
        }
        else
        {
            if (_selfDestroyCoroutine != null)
            {
                StopCoroutine(_selfDestroyCoroutine);
                //Debug.Log($"<color=cyan>{m_currentWeaponData.Name}</color> <color=green>cancel Self Destroy</color>");
            }
        }
    }

    public bool GetDestroyOnEquip()
    {
        return _destroyOnEquip;
    }

    public void DisableDestroyOnEquip()
    {
        _destroyOnEquip = true;
    }

    public void DestroyWeapon()
    {
        Destroy(gameObject);
    }

    public IEnumerator SelfDestroyRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // ======================================== IInteractable ======================================== //

    public string GetInteractPrompt()
    {
        if (!_allowInteract) return "";

        return $"{GlobalData.Prompts.Interact} <color=#08FFFA>{_currentWeaponData.Name}</color>";
    }

    public void Interact(PlayerInteract playerInteract)
    {
        if (!_allowInteract) return;

        playerInteract.InteractWeapon(this);
        OnInteracted?.Invoke();
    }

    public void SetInteractAllowance(bool allow)
    {
        _allowInteract = allow;
    }


    // ======================================== IPriceable ======================================== //

    public string GetPricePrompt()
    {
        if (!_allowInteract) return "";

        if (!_destroyOnEquip)
            return $"{GlobalData.Prompts.Price} <color=#FFCD20>{_currentWeaponData.Price}$</color>";
        else
            return "";
    }


    // ======================================== IWeapon ======================================== //
    
    public IWeaponLogic GetLogic()
    {
        return _currentWeaponLogic;
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
            if (!_weaponServices.ContainsKey(type))
            {
                _weaponServices.Add(type, service);
            }

            // Register service also by his interface to dictionary
            var interfaces = type.GetInterfaces();
            foreach (var i in interfaces)
            {
                if (!_weaponServices.ContainsKey(i))
                {
                    _weaponServices.Add(i, service);
                }
            }
        }
    }

    private void InitAllServices()
    {
        // Distinct = Initiate all services BUT to initiate same service twice (service added to list as BaseWeaponService Or Class name)
        foreach (var service in _weaponServices.Values.OfType<BaseWeaponService>().Distinct())
        {
            service.GetWeaponManager(this);
            service.Init();
        }
    }

    // Where T : class = Ensures T is a class and not a ValueType (like int), allowing the function to return null safely
    public T GetService<T>() where T : class
    {
        if (!_weaponServices.ContainsKey(typeof(T)))
        {
            return null;
        }
        return (T)_weaponServices[typeof(T)];
    }

    private void OnDrawGizmos()
    {
        IWeaponLogic logic = GetComponent<WeaponManager>()?.GetLogic();

        if (logic is ThrowableWeaponLogic throwableLogic)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, throwableLogic.m_data.Radius);
        }
    }
}
