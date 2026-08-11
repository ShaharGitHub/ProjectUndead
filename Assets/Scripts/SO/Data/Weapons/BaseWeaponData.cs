using UnityEngine;

public enum WeaponTypes { Range, Melee, Throwable }

public abstract class BaseWeaponData : ScriptableObject
{
    public Sprite Icon;
    public string Name;
    public WeaponTypes Type;
    public GameObject Prefab;
    public float Damage;

    public abstract IWeaponLogic CreateWeapon();
}
