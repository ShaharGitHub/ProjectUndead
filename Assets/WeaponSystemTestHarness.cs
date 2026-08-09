using UnityEngine;

public class WeaponSystemTestHarness : MonoBehaviour
{
    [SerializeField] private WeaponManager m_weaponManager;
    [SerializeField] private BaseWeaponData m_alternateData; // נשק שני לבדיקת SetData
    [SerializeField] private PlayerCombat m_targetCombat;

    [ContextMenu("Test - Swap Data (simulate Mystery Box)")]
    private void TestSwapData()
    {
        m_weaponManager.SetData(m_alternateData);
    }

    [ContextMenu("Test - Interact/Pickup")]
    private void TestInteract()
    {
        var pickup = m_weaponManager.GetService<WeaponPickup>();
        if (pickup == null)
        {
            Debug.LogError("WeaponPickup service not found!");
            return;
        }
        pickup.PickupWeapon(m_targetCombat);
    }

    [ContextMenu("Test - Fire Weapon")]
    private void TestFire()
    {
        if (m_targetCombat.m_currentWeapon == null)
        {
            Debug.LogError("Player has no weapon equipped!");
            return;
        }
        m_targetCombat.m_currentWeapon.Use();
    }
}
