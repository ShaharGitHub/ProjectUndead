using System;
using UnityEngine;

public class PlayerCombat : BasePlayerService
{
    // Input data
    private InputData m_currentInputData;

    private WeaponManager m_currentWeapon;

    //private bool m_canUseWeapon = true;


    private void OnDisable()
    {
        m_playerManager.OnPlayerInputUpdated -= HandleInput;
        m_playerManager.OnWeaponSwitched -= HandleWeaponSwitched;
    }

    public override void Init()
    {
        base.Init();

        if (m_playerManager == null) return;

        m_playerManager.OnPlayerInputUpdated += HandleInput;
        m_playerManager.OnWeaponSwitched += HandleWeaponSwitched;
    }

    private void HandleInput(InputData inputData)
    {
        m_currentInputData = inputData;
    }

    private void HandleWeaponSwitched(WeaponManager weapon)
    {
        m_currentWeapon = weapon;
    }

    private void Update()
    {
        if (m_currentInputData == null) return;

        AimWeapon(m_currentInputData.ADS);

        if (m_currentInputData.Shoot)
        {
            UseWeapon();
        }
    }

    private void AimWeapon(bool isAiming)
    {
        if (m_currentWeapon == null) return;

        IWeaponLogic logic = m_currentWeapon.GetLogic();
        if (logic != null && logic is RangeWeaponLogic rangeLogic)
        {
            m_playerManager.HandleWeaponAiming(isAiming, rangeLogic.m_data.AdsPosition);
        }
    }

    private void UseWeapon()
    {
        if (m_currentWeapon == null) return;

        //if (!m_canUseWeapon) return;

        // Use weapon
        m_currentWeapon?.GetLogic()?.Use(m_currentWeapon);
    }
}
