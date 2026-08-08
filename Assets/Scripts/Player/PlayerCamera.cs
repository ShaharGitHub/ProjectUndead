using UnityEngine;

public class PlayerCamera : BasePlayerService
{
    public Transform m_eyesTransform;

    public Transform GetEyesPosition()
    {
        return m_eyesTransform;
    }
}
