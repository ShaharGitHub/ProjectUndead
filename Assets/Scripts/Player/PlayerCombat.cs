using System;
using UnityEngine;

public class PlayerCombat : BasePlayerService
{
    // Input data
    private InputData m_currentInputData;

    private WeaponManager m_currentWeapon;
    private bool m_wasShootPressedLastFrame = false;


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

        FireRateWeapon();
    }

    private void AimWeapon()
    {
        if (m_currentWeapon == null)
            return;

        IWeaponLogic logic = m_currentWeapon.GetLogic();
        if (logic != null && logic is RangeWeaponLogic rangeLogic)
        {
            m_playerManager.HandleWeaponAiming(m_currentInputData.ADS, rangeLogic.m_data.AdsPosition);
        }
    }

    private void UseWeapon()
    {
        if (m_currentWeapon == null)
            return;

        IWeaponLogic logic = m_currentWeapon?.GetLogic();
        if (logic == null)
            return;

        bool isShootingNow = m_currentInputData.Shoot;

        if (isShootingNow)
        {
            logic.Use(m_currentWeapon);
        }
        else if (m_wasShootPressedLastFrame)
        {
            // Release trigger
            logic.OnReleaseTrigger();
            
            // Remove throwable from slots
            if (logic is ThrowableWeaponLogic throwableWeapon)
            {
                m_playerManager.HandleWeaponSwitched(null);
            }
        }

        m_wasShootPressedLastFrame = isShootingNow;
    }

    private void ReloadWeapon()
    {
        if (m_currentWeapon == null || !m_currentInputData.Reload)
            return;

        IWeaponLogic logic = m_currentWeapon.GetLogic();
        if (logic != null && logic is IAmmoWeapon ammoWeapon)
        {
            ammoWeapon.TryReload(m_currentWeapon);
        }
    }

    private void FireRateWeapon()
    {
        if (m_currentWeapon == null || !m_currentInputData.FireRate)
            return;

        IWeaponLogic logic = m_currentWeapon.GetLogic();
        if (logic != null && logic is RangeWeaponLogic rangeWeapon)
        {
            rangeWeapon.ChangeFireMode();
        }
    }
}
