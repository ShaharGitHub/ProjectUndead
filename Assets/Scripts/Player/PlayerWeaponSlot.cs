using UnityEngine;

public class PlayerWeaponSlot : BasePlayerService
{
    // Input data
    private InputData m_currentInputData;

    [Header("Weapon Prefab:")]
    [SerializeField] private GameObject m_weaponPrefab;

    [Header("Weapon Hand:")]
    [SerializeField] private Transform m_hand;

    [Header("Slots:")]
    [Min(1)][SerializeField] private int m_maxSlots; // Max weapon amout to hold
    [SerializeField] private WeaponManager[] m_weaponSlots;
    private int m_activeSlotIndex = 0;
    private int m_scrollInput;


    private void Awake()
    {
        m_weaponSlots = new WeaponManager[m_maxSlots];
    }

    private void OnDisable()
    {
        //m_playerManager.OnPlayerInputUpdated -= HandleInput;
        m_playerManager.OnPlayerLocomotionInputUpdated -= HandleInput; // <-- Can cause low FPS !!!
        m_playerManager.OnWeaponSwitched -= HandleWeaponSwitched;
    }

    public override void Init()
    {
        base.Init();

        if (m_playerManager == null) return;

        //m_playerManager.OnPlayerInputUpdated += HandleInput;
        m_playerManager.OnPlayerLocomotionInputUpdated += HandleInput; // <-- Can cause low FPS !!!
        m_playerManager.OnWeaponSwitched += HandleWeaponSwitched;
    }

    private void HandleInput(InputData inputData)
    {
        m_currentInputData = inputData;
        m_scrollInput = m_currentInputData.Scroll;
    }

    private void HandleWeaponSwitched(WeaponManager weapon)
    {
        if (weapon == null)
            RemoveWeapon();
    }

    private void Update()
    {
        if (m_currentInputData == null) return;

        SwitchWeaponIndex(m_scrollInput);

        if (m_currentInputData.Drop)
        {
            RemoveWeapon();
        }
    }

    public void IsWeaponExist(WeaponManager pickedWeapon)
    {
        WeaponManager currentWeapon = null;

        // Get picked weapon name
        string pickedWeaponName = pickedWeapon.GetLogic().GetData().Name;

        // Check if weapon exist in the slots
        foreach (var weapon in m_weaponSlots)
        {
            if (weapon == null)
                continue;

            if (weapon.GetLogic().GetData().Name == pickedWeaponName)
            {
                currentWeapon = weapon;
                break;
            }
        }

        // Weapon found
        if (currentWeapon != null)
        {
            // Ammo weapon
            if (currentWeapon.GetLogic() is IAmmoWeapon ammoWeapon)
            {
                ammoWeapon.TryAddAmmo();
            }
        }
        // Weapon not found
        else
        {
            AddWeapon(pickedWeapon);
        }
    }

    private int GetSlot()
    {
        for (int i = 0; i < m_weaponSlots.Length; i++)
        {
            if (m_weaponSlots[i] == null)
                return i;
        }
        return m_activeSlotIndex;
    }

    public void AddWeapon(WeaponManager newWeapon)
    {
        // Get slot for new weapon
        int slot = GetSlot();

        // Drop held weapon if no slots available
        if (slot == m_activeSlotIndex && m_weaponSlots[m_activeSlotIndex] != null)
            RemoveWeapon();

        // Hide weapon if not selected
        if (m_weaponSlots[m_activeSlotIndex] != null)
            SwitchWeaponObject(false);

        // Get new weapon to current slot
        Transform weaponParent = m_hand != null ? m_hand : transform;
        WeaponManager equippedWeapon = newWeapon.GetLogic().Equip(newWeapon, m_weaponPrefab, weaponParent, m_playerManager.m_eyesCamera);
        m_weaponSlots[slot] = equippedWeapon;
        m_activeSlotIndex = slot;

        // Show selected waepon
        SwitchWeaponObject(true);
    }

    public void RemoveWeapon()
    {
        if (m_weaponSlots.Length == 0 || m_weaponSlots[m_activeSlotIndex] == null) return;

        // Get current weapon
        WeaponManager currentWeapon = m_weaponSlots[m_activeSlotIndex];

        // Active drop logic
        currentWeapon.GetLogic().Drop(currentWeapon);

        // Empty the slot
        m_weaponSlots[m_activeSlotIndex] = null;

        // Search for next active weapon
        int newIndex = -1;
        for (int i = 0; i < m_weaponSlots.Length; i++)
        {
            if (m_weaponSlots[i] != null)
            {
                newIndex = i;
                break;
            }
        }

        // Change active slot index
        m_activeSlotIndex = newIndex != -1 ? newIndex : 0;

        // Show current weapon (if having weapon)
        if (m_weaponSlots[m_activeSlotIndex] != null)
            SwitchWeaponObject(true);
        else
            UpdateWeaponSwitch(null);
    }

    private void SwitchWeaponIndex(int side)
    {
        if (side != 0 && m_weaponSlots.Length > 0)
        {
            // Check how much weapon the player hold
            int weaponsCount = 0;
            foreach (var slot in m_weaponSlots)
            {
                if (slot != null)
                    weaponsCount++;
            }
            // No need to switch if only have 1 weapon
            if (weaponsCount <= 1) return;

            // Hide curernt weapon
            SwitchWeaponObject(false);

            if (side > 0)
            {
                m_activeSlotIndex++;
                if (m_activeSlotIndex >= m_weaponSlots.Length)
                    m_activeSlotIndex = 0;
            }
            else
            {
                m_activeSlotIndex--;
                if (m_activeSlotIndex < 0)
                    m_activeSlotIndex = m_weaponSlots.Length -1;
            }

            // Show next weapon
            SwitchWeaponObject(true);
        }

        // Reset scroll input
        m_scrollInput = 0;
    }

    private void SwitchWeaponObject(bool stat)
    {
        WeaponManager weapon = m_weaponSlots[m_activeSlotIndex];
        weapon.gameObject.SetActive(stat);

        UpdateWeaponSwitch(weapon);
    }

    private void UpdateWeaponSwitch(WeaponManager weapon)
    {
        m_playerManager.HandleWeaponSwitched(weapon);
    }
}
