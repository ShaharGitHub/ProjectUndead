using UnityEngine;

public enum WeaponTypes { Range, Melee, Throwable }

public abstract class BaseWeaponData : ScriptableObject
{
    [Header("General Stats:")]
    public Sprite Icon;
    public string Name;
    public WeaponTypes Type;
    public GameObject Prefab;
    public float Damage;

    [Header("Economy:")]
    public int Price;

    public abstract IWeaponLogic CreateWeapon();
}
