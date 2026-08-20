using UnityEngine;

public class PlayerWeaponSlot : BasePlayerService
{
    // Input data
    private InputData m_currentInputData;

    [Min(1)][SerializeField] private int m_maxSlots; // Max weapon amout to hold
    [SerializeField] private IWeapon[] m_currentWeapons;
    private int m_activeWeaponIndex = 0;
    private int m_switchWeaponIndex;


    private void Awake()
    {
        m_currentWeapons = new IWeapon[m_maxSlots];
    }

    private void OnDisable()
    {
        m_playerManager.OnPlayerInputUpdated -= HandleInput;
    }

    public override void Init()
    {
        base.Init();

        if (m_playerManager == null) return;

        m_playerManager.OnPlayerInputUpdated += HandleInput;
    }

    private void HandleInput(InputData inputData)
    {
        m_currentInputData = inputData;
        m_switchWeaponIndex = m_currentInputData.Scroll;
    }

    private void Update()
    {
        if (m_currentInputData == null) return;

        //Debug.Log(m_switchWeaponIndex);
        SwitchWeaponIndex(m_switchWeaponIndex);

        if (m_currentInputData.Drop)
        {
            RemoveWeapon();
        }
    }

    private int GetSlot()
    {
        for (int i = 0; i < m_currentWeapons.Length; i++)
        {
            if (m_currentWeapons[i] == null)
                return i;
        }
        return m_activeWeaponIndex;
    }

    public void AddWeapon(WeaponManager newWeapon)
    {
        // Get slot for new weapon
        int slot = GetSlot();

        // Drop held weapon if no slots available
        if (slot == m_activeWeaponIndex && m_currentWeapons[m_activeWeaponIndex] != null)
            RemoveWeapon();

        // Hide weapon if not selected
        if (m_currentWeapons[m_activeWeaponIndex] != null)
            SwitchWeaponObject(false);

        // Get new weapon to current slot
        WeaponManager equippedWeapon = newWeapon.GetLogic().Equip(newWeapon, transform);
        m_currentWeapons[slot] = equippedWeapon;
        m_activeWeaponIndex = slot;

        // Show selected waepon
        SwitchWeaponObject(true);
    }

    private void SwitchWeaponIndex(int side)
    {
        if (side != 0 && m_currentWeapons.Length != 0 && m_currentWeapons[m_activeWeaponIndex] != null)
        {
            SwitchWeaponObject(false);

            if (side > 0)
            {
                m_activeWeaponIndex++;
                if (m_activeWeaponIndex >= m_currentWeapons.Length)
                    m_activeWeaponIndex--;
            }
            else
            {
                m_activeWeaponIndex--;
                if (m_activeWeaponIndex < 0)
                    m_activeWeaponIndex++;
            }

            SwitchWeaponObject(true);
        }

        m_switchWeaponIndex = 0;
    }

    private void SwitchWeaponObject(bool stat)
    {
        WeaponManager weapon = m_currentWeapons[m_activeWeaponIndex] as WeaponManager;
        weapon.gameObject.SetActive(stat);

        m_playerManager.HandleWeaponSwitched(weapon);
    }

    public void RemoveWeapon()
    {
        if (m_currentWeapons.Length == 0 || m_currentWeapons[m_activeWeaponIndex] == null) return;

        IWeapon currentWeapon = m_currentWeapons[m_activeWeaponIndex];
        currentWeapon.GetLogic().Drop(currentWeapon as WeaponManager);
        m_currentWeapons[m_activeWeaponIndex] = null;
        m_activeWeaponIndex = Mathf.Max(m_activeWeaponIndex -= 1, 0);

        if (m_currentWeapons[m_activeWeaponIndex] != null)
            SwitchWeaponObject(true);
    }
}
