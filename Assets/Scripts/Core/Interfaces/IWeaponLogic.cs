using UnityEngine;

public interface IWeaponLogic
{
    public BaseWeaponData GetData();
    public WeaponManager Equip(WeaponManager equipWeapon, Transform handSlot);
    public void Use(WeaponManager weapon);
    public void Drop(WeaponManager weapon);
}
