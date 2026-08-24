using UnityEngine;

public class PlayerCamera : BasePlayerService
{
    // Input data
    private InputData m_currentInputData;

    [Header("FPS:")]
    public Transform m_fpsEyesTransform;

    [Header("Settings:")]
    public float m_cameraAdsFov;
    private Vector3 m_handAdsPosition;
    private float m_cameraDefaultFov;
    private Vector3 m_handDefaultPosition;
    private bool m_isAiming = false;

    [Header("References:")]
    public Camera m_eyesCamera;
    public Transform m_weaponHand;

    // DEBUG
    public bool m_forceADS;


    private void OnDisable()
    {
        m_playerManager.OnWeaponAiming -= HandleAim;
    }

    public override void Init()
    {
        base.Init();

        if (m_playerManager == null) return;

        m_playerManager.OnWeaponAiming += HandleAim;

        SetDefaultValues();
    }

    private void HandleAim(bool isAiming, Vector3 weaponAdsPos)
    {
        m_isAiming = isAiming;
        m_handAdsPosition = weaponAdsPos;
    }

    private void Update()
    {
        if (m_forceADS)
            m_isAiming = true;

        AimWeapon(m_isAiming);
    }

    private void SetDefaultValues()
    {
        m_cameraDefaultFov = m_eyesCamera.fieldOfView;
        m_handDefaultPosition = m_weaponHand.localPosition;
    }

    public void AimWeapon(bool isADS)
    {
        if (m_eyesCamera == null || m_weaponHand == null) return;

        // Set FOV
        float targetFOV = isADS ? m_cameraAdsFov : m_cameraDefaultFov;
        m_eyesCamera.fieldOfView = Mathf.Lerp(m_eyesCamera.fieldOfView, targetFOV, 0.25f);

        // Set hand position
        Vector3 targetHandPos = isADS ? m_handAdsPosition : m_handDefaultPosition;
        m_weaponHand.localPosition = Vector3.Lerp(m_weaponHand.localPosition, targetHandPos, 0.25f);
    }

    public Transform GetFpsEyes()
    {
        return m_fpsEyesTransform;
    }
}
