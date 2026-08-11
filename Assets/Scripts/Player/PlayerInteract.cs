using UnityEngine;

public class PlayerInteract : BasePlayerService
{
    // Input data
    private InputData m_currentInputData;

    // Weapon
    [SerializeField] private float m_rayRange;

    // References
    private Camera eyesCamera;


    private void OnDisable()
    {
        m_playerManager.OnPlayerInputUpdated -= HandleInput;
    }

    public override void Init()
    {
        base.Init();

        if (m_playerManager == null) return;

        eyesCamera = m_playerManager.GetComponentInChildren<Camera>();

        m_playerManager.OnPlayerInputUpdated += HandleInput;
    }

    private void HandleInput(InputData inputData)
    {
        m_currentInputData = inputData;
    }

    private void Update()
    {
        if (m_currentInputData == null) return;

        if (m_currentInputData.Interact)
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (eyesCamera == null) return;

        // Create ray exit point
        Vector3 rayOrigin = eyesCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

        // Check for hits
        if (Physics.Raycast(rayOrigin, eyesCamera.transform.forward, out RaycastHit hit, m_rayRange))
        {
            //Debug.Log($"Hit: {hit.transform.name}");

            // Interact Weapon
            if (hit.transform.TryGetComponent<IWeapon>(out IWeapon weapon))
            {
                PlayerWeaponSlot weaponSlots = m_playerManager.GetService<PlayerWeaponSlot>();
                weaponSlots?.AddWeapon(hit.transform.GetComponent<WeaponManager>());
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (eyesCamera == null) return;

        Vector3 rayOrigin = eyesCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin, eyesCamera.transform.forward * m_rayRange);
    }
}
