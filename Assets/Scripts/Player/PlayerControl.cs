using UnityEngine;

public class PlayerControl : BasePlayerService
{
    private InputData m_currentInputData;

    private Transform m_rootCharacter;
    private Rigidbody m_rootRigidbody;

    private float m_eyesRotation;


    private void OnDisable()
    {
        m_playerManager.OnPlayerInputUpdated -= HandleInput;
    }

    public override void Init()
    {
        base.Init();

        if (m_playerManager == null) return;

        m_playerManager.OnPlayerInputUpdated += HandleInput;

        m_rootCharacter = m_playerManager.transform;
        m_rootRigidbody = m_rootCharacter.GetComponent<Rigidbody>();
    }

    private void HandleInput(InputData inputData)
    {
        m_currentInputData = inputData;
    }

    private void Update()
    {
        if (m_currentInputData == null) return;

        Movement();
        Look();
    }

    private void Movement()
    {
        // Movement diraction from input
        Vector3 movementDir = new Vector3(m_currentInputData.Movement.x, 0, m_currentInputData.Movement.y);

        // Current transform position with input diraction
        Vector3 targetMovement = transform.forward * movementDir.z + transform.right * movementDir.x;

        // Update movement
        //m_rootCharacter.Translate(movement * m_playerManager.PlayerData.MovementSpeed * Time.deltaTime);
        m_rootRigidbody.linearVelocity = targetMovement * (m_playerManager.PlayerData.MovementSpeed * 100) * Time.deltaTime;
    }

    private void Look()
    {
        // Rotate body (Y axis)
        float yRotation = m_currentInputData.Look.x * m_playerManager.PlayerData.LookSpeed * Time.deltaTime;
        m_playerManager.transform.Rotate(0, yRotation, 0);

        // Find eyes transform
        Transform eyes = m_playerManager.GetService<PlayerCamera>().GetEyesPosition();
        if (eyes == null) return;

        // Rotate eyes (X Axis)
        float xRotation = m_currentInputData.Look.y * m_playerManager.PlayerData.LookSpeed * Time.deltaTime;
        m_eyesRotation -= xRotation;
        m_eyesRotation = Mathf.Clamp(m_eyesRotation, -90, 90);
        eyes.localRotation = Quaternion.Euler(m_eyesRotation, 0, 0);
    }
}
