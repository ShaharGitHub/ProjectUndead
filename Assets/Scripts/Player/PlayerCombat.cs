using System;
using UnityEngine;

public class PlayerCombat : BasePlayerService
{
    // Input data
    private InputData m_currentInputData;

    private WeaponManager m_currentWeapon;


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

        AimWeapon();

        UseWeapon();

        ReloadWeapon();
    }

    private void AimWeapon()
    {
        if (m_currentWeapon == null) return;

        IWeaponLogic logic = m_currentWeapon.GetLogic();
        if (logic != null && logic is RangeWeaponLogic rangeLogic)
        {
            m_playerManager.HandleWeaponAiming(m_currentInputData.ADS, rangeLogic.m_data.AdsPosition);
        }
    }

    private void UseWeapon()
    {
        if (m_currentWeapon == null) return;

        IWeaponLogic logic = m_currentWeapon?.GetLogic();
        if (logic == null)
            return;

        if (m_currentInputData.Shoot)
        {
            logic.Use(m_currentWeapon);
        }
        else
        {
            // Release trigger (for semi weapon like pistols)
            if (logic is RangeWeaponLogic rangeWeapon)
                rangeWeapon.OnReleaseTrigger();
        }
    }

    private void ReloadWeapon()
    {
        if (m_currentWeapon == null || !m_currentInputData.Reload) return;

        IWeaponLogic logic = m_currentWeapon.GetLogic();
        if (logic != null && logic is IAmmoWeapon ammoWeapon)
        {
            ammoWeapon.TryReload(m_currentWeapon);
        }
    }
}
