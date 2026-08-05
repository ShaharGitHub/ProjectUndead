using UnityEngine;

public abstract class BasePlayerService : MonoBehaviour
{
    [SerializeField] protected PlayerManager m_playerManager;

    public virtual void GetPlayerManager(PlayerManager playerManager)
    {
        m_playerManager = playerManager;
    }

    public virtual void Init() { }
}
