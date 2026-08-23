using UnityEngine;

public class WeaponSystemTestHarness : MonoBehaviour
{
    [SerializeField] private WeaponManager m_weaponManager;  // Weapon manager
    [SerializeField] private BaseWeaponData m_alternateData; // Different Data to switch for
    [SerializeField] private PlayerManager m_targetManager;  // Player Manager
    [SerializeField] private Transform m_handSlot;  // Player Hand

    [ContextMenu("Test - Swap Data (simulate Mystery Box)")]
    private void TestSwapData()
    {
        m_weaponManager.SetData(m_alternateData);
    }

    //[ContextMenu("Test - Equip")]
    //private void TestInteract()
    //{
    //    var weaponLogic = m_weaponManager.GetLogic();
    //    if (weaponLogic == null)
    //    {
    //        Debug.LogError("WeaponPickup service not found!");
    //        return;
    //    }
    //    //weaponLogic.Equip(m_weaponManager, m_handSlot);
    //}

    //[ContextMenu("Test - Drop")]
    //private void TestDrop()
    //{
    //    var weaponLogic = m_weaponManager.GetLogic();
    //    if (weaponLogic == null)
    //    {
    //        Debug.LogError("WeaponPickup service not found!");
    //        return;
    //    }
    //    weaponLogic.Drop(m_weaponManager);
    //}

    //[ContextMenu("Test - Fire Weapon")]
    //private void TestFire()
    //{
    //    if (m_targetCombat.m_currentWeapon == null)
    //    {
    //        Debug.LogError("Player has no weapon equipped!");
    //        return;
    //    }
    //    m_targetCombat.m_currentWeapon.Use();
    //}
}
