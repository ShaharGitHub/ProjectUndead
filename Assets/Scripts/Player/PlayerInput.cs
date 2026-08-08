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

    private Vector2 lastSentMovement;
    private Vector2 lastSentLook;

    // Input data to send by event
    public InputData inputData;

    // Event update input changes
    public event Action<InputData> OnInputUpdated;


    private void Awake()
    {
        inputData = new InputData();

        control = new PlayerInput_Actions();

        movementAction = control?.Player.Movement;
        lookAction = control?.Player.Look;
    }

    private void OnEnable()
    {
        control?.Enable();

        control.Player.Sprint.performed += ctx => { inputData.Sprint = true; TriggerInputEvent(); };
        control.Player.Sprint.canceled += ctx => { inputData.Sprint = false; TriggerInputEvent(); };

        control.Player.Jump.performed += ctx => { inputData.Jump = true; TriggerInputEvent(); };

        control.Player.ADS.performed += ctx => { inputData.ADS = true; TriggerInputEvent(); };
        control.Player.ADS.canceled += ctx => { inputData.ADS = false; TriggerInputEvent(); };

        control.Player.Shoot.performed += ctx => { inputData.Shoot = true; TriggerInputEvent(); };
        control.Player.Shoot.canceled += ctx => { inputData.Shoot = false; TriggerInputEvent(); };

        control.Player.Reload.performed += ctx => { inputData.Reload = true; TriggerInputEvent(); };
        control.Player.Melee.performed += ctx => { inputData.Melee = true; TriggerInputEvent(); };
        control.Player.Grenade.performed += ctx => { inputData.Grenade = true; TriggerInputEvent(); };
        control.Player.Interact.performed += ctx => { inputData.Interact = true; TriggerInputEvent(); };
    }

    private void OnDisable()
    {
        control.Player.Sprint.performed -= ctx => { inputData.Sprint = true; TriggerInputEvent(); };
        control.Player.Sprint.canceled -= ctx => { inputData.Sprint = false; TriggerInputEvent(); };

        control.Player.Jump.performed -= ctx => { inputData.Jump = true; TriggerInputEvent(); };

        control.Player.ADS.performed -= ctx => { inputData.ADS = true; TriggerInputEvent(); };
        control.Player.ADS.canceled -= ctx => { inputData.ADS = false; TriggerInputEvent(); };

        control.Player.Shoot.performed -= ctx => { inputData.Shoot = true; TriggerInputEvent(); };
        control.Player.Shoot.canceled -= ctx => { inputData.Shoot = false; TriggerInputEvent(); };

        control.Player.Reload.performed -= ctx => { inputData.Reload = true; TriggerInputEvent(); };
        control.Player.Melee.performed -= ctx => { inputData.Melee = true; TriggerInputEvent(); };
        control.Player.Grenade.performed -= ctx => { inputData.Grenade = true; TriggerInputEvent(); };
        control.Player.Interact.performed -= ctx => { inputData.Interact = true; TriggerInputEvent(); };

        control?.Disable();
    }

    private void Update()
    {
        ReadContinuousInputs();
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

            // Check player last movement
            if (currentMovement != lastSentMovement || currentLook != lastSentLook)
            {
                inputData.Movement = currentMovement;
                inputData.Look = currentLook;

                lastSentMovement = currentMovement;
                lastSentLook = currentLook;

                TriggerInputEvent();
            }
        }
    }

    private void ResetInput()
    {
        inputData.Jump = false;
        inputData.Reload = false;
        inputData.Melee = false;
        inputData.Grenade = false;
        inputData.Interact = false;
    }

    private void TriggerInputEvent()
    {
        OnInputUpdated?.Invoke(inputData);
    }
}
