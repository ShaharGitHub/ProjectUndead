using UnityEngine;

public class PlayerInteract : BasePlayerService
{
    // Input data
    private InputData m_currentInputData;

    private void OnDisable()
    {
        m_playerManager.OnPlayerInputUpdated -= HandleInput;
        // Add event to weapon pickup (Set Weapon to combat script ?)
    }

    public override void Init()
    {
        base.Init();

        if (m_playerManager == null) return;

        m_playerManager.OnPlayerInputUpdated += HandleInput;
        // Add event to weapon pickup (Set Weapon to combat script ?)
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
        // Check reycast
    }
}
