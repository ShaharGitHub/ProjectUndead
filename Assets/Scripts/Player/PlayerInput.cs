using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : BasePlayerService, IInputProvider
{
    // Input control map
    private PlayerInput_Actions control;

    // Vector2 input actions
    private InputAction movementAction;
    private InputAction lookAction;
    private InputAction scrollAction;

    private Vector2 lastSentMovement;
    private Vector2 lastSentLook;
    private int lastSentScroll;

    // Input data to send by event
    public InputData inputData;

    // Event update input changes
    public event Action<InputData> OnLocomotionInputUpdated;
    public event Action<InputData> OnInputUpdated;


    private void Awake()
    {
        inputData = new InputData();
        control = new PlayerInput_Actions();

        movementAction = control?.Player.Movement;
        lookAction = control?.Player.Look;
        scrollAction = control?.Player.Scroll;
    }

    private void OnEnable()
    {
        control?.Enable();

        control.Player.Sprint.performed += OnSprintPerformed;
        control.Player.Sprint.canceled += OnSprintCanceled;

        control.Player.Jump.performed += OnJumpPerformed;
        control.Player.Drop.performed += OnDropPerformed;

        control.Player.ADS.performed += OnADSPerformed;
        control.Player.ADS.canceled += OnADSCanceled;

        control.Player.Shoot.performed += OnShootPerformed;
        control.Player.Shoot.canceled += OnShootCanceled;

        control.Player.Reload.performed += OnReloadPerformed;
        control.Player.FireRate.performed += OnFireRatePerformed;
        control.Player.Melee.performed += OnMeleePerformed;
        control.Player.Grenade.performed += OnGrenadePerformed;
        control.Player.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        control.Player.Sprint.performed -= OnSprintPerformed;
        control.Player.Sprint.canceled -= OnSprintCanceled;

        control.Player.Jump.performed -= OnJumpPerformed;
        control.Player.Drop.performed -= OnDropPerformed;

        control.Player.ADS.performed -= OnADSPerformed;
        control.Player.ADS.canceled -= OnADSCanceled;

        control.Player.Shoot.performed -= OnShootPerformed;
        control.Player.Shoot.canceled -= OnShootCanceled;

        control.Player.Reload.performed -= OnReloadPerformed;
        control.Player.FireRate.performed -= OnFireRatePerformed;
        control.Player.Melee.performed -= OnMeleePerformed;
        control.Player.Grenade.performed -= OnGrenadePerformed;
        control.Player.Interact.performed -= OnInteractPerformed;

        control?.Disable();
    }

    private void OnSprintPerformed(InputAction.CallbackContext ctx) { inputData.Sprint = true; TriggerInputEvent(); }
    private void OnSprintCanceled(InputAction.CallbackContext ctx) { inputData.Sprint = false; TriggerInputEvent(); }
    private void OnJumpPerformed(InputAction.CallbackContext ctx) { inputData.Jump = true; TriggerInputEvent(); }
    private void OnDropPerformed(InputAction.CallbackContext ctx) { inputData.Drop = true; TriggerInputEvent(); }
    private void OnADSPerformed(InputAction.CallbackContext ctx) { inputData.ADS = true; TriggerInputEvent(); }
    private void OnADSCanceled(InputAction.CallbackContext ctx) { inputData.ADS = false; TriggerInputEvent(); }
    private void OnShootPerformed(InputAction.CallbackContext ctx) { inputData.Shoot = true; TriggerInputEvent(); }
    private void OnShootCanceled(InputAction.CallbackContext ctx) { inputData.Shoot = false; TriggerInputEvent(); }
    private void OnReloadPerformed(InputAction.CallbackContext ctx) { inputData.Reload = true; TriggerInputEvent(); }
    private void OnFireRatePerformed(InputAction.CallbackContext ctx) { inputData.FireRate = true; TriggerInputEvent(); }
    private void OnMeleePerformed(InputAction.CallbackContext ctx) { inputData.Melee = true; TriggerInputEvent(); }
    private void OnGrenadePerformed(InputAction.CallbackContext ctx) { inputData.Grenade = true; TriggerInputEvent(); }
    private void OnInteractPerformed(InputAction.CallbackContext ctx) { inputData.Interact = true; TriggerInputEvent(); }

    private void Update()
    {
        ReadContinuousInputs();
        //Debug_CheckInput();
    }

    private void LateUpdate()
    {
        ResetInput();
    }

    private void ReadContinuousInputs()
    {
        if (movementAction != null && lookAction != null)
        {
            Vector2 currentMovement = movementAction.ReadValue<Vector2>();
            Vector2 currentLook = lookAction.ReadValue<Vector2>();

            int currentScroll = 0;
            if (scrollAction != null)
            {
                float scrollValue = scrollAction.ReadValue<Vector2>().y;
                if (scrollValue > 0f) currentScroll = 1;
                else if (scrollValue < 0f) currentScroll = -1;
            }

            // Check player last movement
            if (currentMovement != lastSentMovement || currentLook != lastSentLook || currentScroll != lastSentScroll)
            {
                inputData.Movement = currentMovement;
                inputData.Look = currentLook;
                inputData.Scroll = currentScroll;

                lastSentMovement = currentMovement;
                lastSentLook = currentLook;
                lastSentScroll = currentScroll;

                TriggerLocomotionInputEvent();
            }
        }
    }

    private void ResetInput()
    {
        inputData.Jump = false;
        inputData.Drop = false;
        inputData.Reload = false;
        inputData.FireRate = false;
        inputData.Melee = false;
        inputData.Grenade = false;
        inputData.Interact = false;

        inputData.Scroll = 0;
        lastSentScroll = 0;
    }

    private void TriggerInputEvent()
    {
        OnInputUpdated?.Invoke(inputData);
    }

    private void TriggerLocomotionInputEvent()
    {
        OnLocomotionInputUpdated?.Invoke(inputData);
    }

    private void Debug_CheckInput()
    {
        Debug.Log(inputData.Scroll);
    }
}
