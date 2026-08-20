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

        Interact();
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

            // Active interact by pressing "E"
            if (m_currentInputData.Interact)
            {
                // Interact Weapon
                if (hit.transform.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    interactable.Interact(this);
                    Debug.Log(interactable.GetInteractPrompt());
                }
            }
        }
    }

    public void InteractWeapon(WeaponManager weaponManager)
    {
        PlayerWeaponSlot weaponSlots = m_playerManager.GetService<PlayerWeaponSlot>();
        weaponSlots?.AddWeapon(weaponManager);
    }

    private void OnDrawGizmos()
    {
        if (eyesCamera == null) return;

        Vector3 rayOrigin = eyesCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin, eyesCamera.transform.forward * m_rayRange);
    }
}
