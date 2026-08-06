using UnityEngine;

public class PlayerControl : BasePlayerService
{
    private Transform m_mainCharacter;
    private InputData m_currentInputData;


    private void OnDisable()
    {
        m_playerManager.OnPlayerInputUpdated -= HandleInput;
    }

    public override void Init()
    {
        base.Init();

        if (m_playerManager == null) return;

        m_playerManager.OnPlayerInputUpdated += HandleInput;

        m_mainCharacter = m_playerManager.transform;
    }

    private void HandleInput(InputData inputData)
    {
        m_currentInputData = inputData;
    }

    private void FixedUpdate()
    {
        if (m_currentInputData == null) return;

        Movement();
        Look();
    }

    private void Movement()
    {
        Vector3 movement = new Vector3(m_currentInputData.Movement.x, 0, m_currentInputData.Movement.y);

        m_mainCharacter.Translate(movement * m_playerManager.PlayerData.MovementSpeed * Time.fixedDeltaTime);
    }

    private void Look()
    {

    }
}
