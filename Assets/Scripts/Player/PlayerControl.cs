using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerControl : BasePlayerService
{
    // Input data
    private InputData m_currentInputData;

    // Root elements
    private Transform m_rootCharacter;
    private Rigidbody m_rootRigidbody;

    // Eyes reference
    private float m_eyesRotation;

    [Header("Jump Setting:")]
    [SerializeField] private LayerMask m_groundMask;
    //[SerializeField] private float m_groundCheckDistance = 0.2f;
    [SerializeField] private bool m_isGrounded;
    private bool m_applyJump;


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

        if (inputData.Jump)
            m_applyJump = true;
    }

    private void FixedUpdate()
    {
        if (m_currentInputData == null) return;

        Movement();

        if (m_applyJump)
        {
            Jump();
            m_applyJump = false;
        }
    }

    private void Update()
    {
        if (m_currentInputData == null) return;
        
        Look();
    }

    private float GetMovementSpeed()
    {
        if (m_currentInputData.Sprint && m_isGrounded)
            return m_playerManager.PlayerData.SprintSpeed;
        else
            return m_playerManager.PlayerData.WalkSpeed;
    }

    private void Movement()
    {
        // Movement diraction from input
        Vector3 movementDir = new Vector3(m_currentInputData.Movement.x, 0, m_currentInputData.Movement.y);

        // Current transform position with input diraction
        Vector3 targetMovement = (transform.forward * movementDir.z + transform.right * movementDir.x) * GetMovementSpeed();

        Vector3 velocity = m_rootRigidbody.linearVelocity;
        velocity.x = targetMovement.x;
        velocity.z = targetMovement.z;

        m_rootRigidbody.linearVelocity = velocity;
    }

    private void Look()
    {
        // Rotate body (Y axis)
        float yRotation = m_currentInputData.Look.x * m_playerManager.PlayerData.LookSpeed * Time.deltaTime;
        m_playerManager.transform.Rotate(0, yRotation, 0);

        // Find eyes transform
        Transform eyes = m_playerManager.GetService<PlayerCamera>().GetFpsEyes();
        if (eyes == null) return;

        // Rotate eyes (X Axis)
        float xRotation = m_currentInputData.Look.y * m_playerManager.PlayerData.LookSpeed * Time.deltaTime;
        m_eyesRotation -= xRotation;
        m_eyesRotation = Mathf.Clamp(m_eyesRotation, -90, 90);
        eyes.localRotation = Quaternion.Euler(m_eyesRotation, 0, 0);
    }

    public void HandleCollision(Collision col)
    {
        float _colLayerIndex = (int)Mathf.Log(m_groundMask.value, 2);

        if (col.transform.gameObject.layer == _colLayerIndex)
        {
            //Debug.Log("Ground");
            m_isGrounded = true;
        }
    }

    private void Jump()
    {
        if (m_isGrounded)
        {
            m_isGrounded = false;

            // Calculate the required velocity to reach the target height based on physics formula: v = sqrt(2 * g * h)
            float gravity = Mathf.Abs(Physics.gravity.y);
            float jumpVelocity = Mathf.Sqrt(2 * gravity * m_playerManager.PlayerData.JumpHeight);

            // Apply immediate velocity change ignoring mass for a consistent jump height
            Vector3 velocity = m_rootRigidbody.linearVelocity;
            velocity.y = jumpVelocity;
            m_rootRigidbody.linearVelocity = velocity;
        }
    }
}
