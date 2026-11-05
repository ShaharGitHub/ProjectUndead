using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBoxManager : MonoBehaviour, IInteractable, IPriceable
{
    private enum BoxStates { Idle, Spinning, WeaponReady }

    [Header("References:")]
    [SerializeField] private WeaponManager _displayWeapon;    // The weapon that floats above the box

    [Header("Settings:")]
    [SerializeField] private List<BaseWeaponData> _weaponPool;
    [SerializeField] private int _price = 950;
    [SerializeField] private float _spinDuration = 4f;        // How long the weapons keep swapping
    [SerializeField] private float _swapInterval = 0.15f;     // Time between each swap
    [SerializeField] private float _grabTime = 8f;            // How long the player has to take the weapon

    private BoxStates _state = BoxStates.Idle;
    private BaseWeaponData _lastWeapon;
    private bool _weaponTaken;


    private void Awake()
    {
        _displayWeapon.OnInteracted += HandleWeaponTaken;
        CloseBox(); // Start closed
    }

    private void OnDestroy()
    {
        if (_displayWeapon != null)
            _displayWeapon.OnInteracted -= HandleWeaponTaken;
    }

    // ======================================== IInteractable ======================================== //

    public string GetInteractPrompt()
    {
        if (_state != BoxStates.Idle) return "";
        return $"{GlobalData.Prompts.Interact} <color=#08FFFA>Mystery Box</color>";
    }

    public void Interact(PlayerInteract playerInteract)
    {
        if (_state != BoxStates.Idle) return;

        if (_weaponPool == null || _weaponPool.Count == 0)
        {
            Debug.LogError($"{name}: weapon pool is empty!");
            return;
        }

        // TODO: charge the player _price here (no money system in the scripts I got)

        StartCoroutine(BoxRoutine());
    }

    public void SetInteractAllowance(bool allow)
    {
        
    }

    // ======================================== IPriceable ======================================== //

    public string GetPricePrompt()
    {
        if (_state != BoxStates.Idle) return "";
        return $"{GlobalData.Prompts.Price} <color=#FFCD20>{_price}$</color>";
    }

    // ======================================== Box flow ======================================== //

    private IEnumerator BoxRoutine()
    {
        // ---- Spin: keep swapping the weapon ---- //
        _state = BoxStates.Spinning;
        _displayWeapon.SetInteractAllowance(false);
        _displayWeapon.gameObject.SetActive(true);

        float spinTimer = 0f;
        while (spinTimer < _spinDuration)
        {
            _displayWeapon.SetData(GetRandomWeapon());
            yield return new WaitForSeconds(_swapInterval);
            spinTimer += _swapInterval;
        }

        // ---- The last weapon stays, the player can take it ---- //
        _state = BoxStates.WeaponReady;
        _weaponTaken = false;
        _displayWeapon.SetInteractAllowance(true);

        float grabTimer = 0f;
        while (!_weaponTaken && grabTimer < _grabTime)
        {
            grabTimer += Time.deltaTime;
            yield return null;
        }

        CloseBox();
    }

    private void HandleWeaponTaken()
    {
        _weaponTaken = true;
    }

    private void CloseBox()
    {
        _displayWeapon.SetInteractAllowance(false);
        _displayWeapon.gameObject.SetActive(false);
        _state = BoxStates.Idle;
    }

    // Random weapon, never the same one twice in a row (so every swap is visible)
    private BaseWeaponData GetRandomWeapon()
    {
        if (_weaponPool.Count == 1)
            return _weaponPool[0];

        BaseWeaponData weapon;
        do
        {
            weapon = _weaponPool[Random.Range(0, _weaponPool.Count)];
        }
        while (weapon == _lastWeapon);

        _lastWeapon = weapon;
        return weapon;
    }
}
