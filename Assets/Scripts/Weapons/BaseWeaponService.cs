using UnityEngine;

public class BaseWeaponService : MonoBehaviour
{
    [SerializeField] protected WeaponManager m_weaponManager;

    public virtual void GetWeaponManager(WeaponManager weaponManager)
    {
        m_weaponManager = weaponManager;
    }

    public virtual void Init() { }
}
