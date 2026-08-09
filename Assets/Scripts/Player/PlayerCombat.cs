using System;
using UnityEngine;

public class PlayerCombat : BasePlayerService
{
    // Input data
    private InputData m_currentInputData;

    public IWeapon m_currentWeapon { get; private set; }

    //private bool m_canUseWeapon = true;


    private void OnDisable()
    {
        m_playerManager.OnPlayerInputUpdated -= HandleInput;
        // Add event to weapon pickup
    }

    public override void Init()
    {
        base.Init();

        if (m_playerManager == null) return;

        m_playerManager.OnPlayerInputUpdated += HandleInput;
        // Add event to weapon pickup
    }

    private void HandleInput(InputData inputData)
    {
        m_currentInputData = inputData;
    }

    public void SetWeapon(IWeapon newWeapon)
    {
        // Pickup weapon
        m_currentWeapon = newWeapon;
    }

    private void Update()
    {
        if (m_currentWeapon == null || m_currentInputData == null) return;

        // If user active power up
        if (m_currentInputData.Shoot)
        {
            UseWeapon();
        }
    }

    private void UseWeapon()
    {
        //if (!m_canUseWeapon) return;

        // Use weapon
        m_currentWeapon.Use();
    }
}
