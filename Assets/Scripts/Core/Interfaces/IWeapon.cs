using UnityEngine;

public enum WeaponTypes { Pistol, AR, SMG, MG, Shotgun, Sniper, Launcher, Granade }

public abstract class IWeapon : MonoBehaviour
{
    public Sprite Icon;
    public string Name;
    public WeaponTypes Type;
    public GameObject Prefab;
    public float Damage;
}
