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

        AimWeapon(m_currentInputData.ADS);

        if (m_currentInputData.Shoot)
        {
            UseWeapon();
        }

        ReloadWeapon(m_currentInputData.Reload);
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

        // Use weapon
        m_currentWeapon?.GetLogic()?.Use(m_currentWeapon);
    }

    private void ReloadWeapon(bool isReloading)
    {
        if (m_currentWeapon == null || !isReloading) return;

        IWeaponLogic logic = m_currentWeapon.GetLogic();
        if (logic != null && logic is IAmmoWeapon ammoWeapon)
        {
            ammoWeapon.TryReload(m_currentWeapon);
        }
    }
}
