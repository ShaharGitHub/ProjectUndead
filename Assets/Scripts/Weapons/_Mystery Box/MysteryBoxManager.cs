using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBoxManager : MonoBehaviour, IInteractable, IPriceable
{
    private enum BoxStates { Idle, Spinning, WeaponReady }

    [Header("References:")]
    [SerializeField] private WeaponManager _displayWeapon;    // The weapon that floats above the box
    [SerializeField] private GameObject[] _lights;

    [Header("Lid:")]
    [SerializeField] private Transform _boxLid;               // The LidPivot object (pivot on the hinge!)
    [SerializeField] private float _lidOpenAngle = -125f;     // Flip the sign if it opens inward
    [SerializeField] private float _lidAnimationDuration = 1.0f;
    private Quaternion _lidClosedRotation;
    private Quaternion _lidOpenRotation;

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
        if (_boxLid != null)
        {
            // Open rotation is relative to the closed one, so the lid keeps its original orientation
            _lidClosedRotation = _boxLid.localRotation;
            _lidOpenRotation = _lidClosedRotation * Quaternion.Euler(_lidOpenAngle, 0f, 0f);
        }

        _displayWeapon.OnInteracted += HandleWeaponTaken;

        // Start closed
        CloseBox();
        StartCoroutine(LidRoutine(false));
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

    [ContextMenu("Active Box")]
    public void DEBUG_TestBoxRoutineFromInspector()
    {
        StartCoroutine(BoxRoutine());
    }

    private IEnumerator BoxRoutine()
    {
        // Open the lid first, only then show the weapon
        yield return LidRoutine(true);
        OpenBox();

        yield return new WaitForSeconds(_lidAnimationDuration);

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

        yield return new WaitForSeconds(0.2f);

        yield return LidRoutine(false);
    }

    private void HandleWeaponTaken()
    {
        _weaponTaken = true;
    }

    private void OpenBox()
    {
        ModifyLightStatus(true);
    }

    private void CloseBox()
    {
        _displayWeapon.SetInteractAllowance(false);
        _displayWeapon.gameObject.SetActive(false);
        _state = BoxStates.Idle;
        ModifyLightStatus(false);
    }

    private IEnumerator LidRoutine(bool toOpen)
    {
        if (_boxLid == null) yield break;

        Quaternion startRotation = _boxLid.localRotation;
        Quaternion targetRotation = toOpen ? _lidOpenRotation : _lidClosedRotation;

        float timer = 0f;
        while (timer < _lidAnimationDuration)
        {
            timer += Time.deltaTime;

            // timer / duration = progress from 0 to 1, SmoothStep makes the movement softer
            float progress = Mathf.SmoothStep(0f, 1f, timer / _lidAnimationDuration);
            _boxLid.localRotation = Quaternion.Slerp(startRotation, targetRotation, progress);

            yield return null;
        }

        _boxLid.localRotation = targetRotation; // Make sure it ends exactly on target
    }

    private void ModifyLightStatus(bool active)
    {
        foreach (GameObject light in _lights)
        {
            light.SetActive(active);
        }
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
