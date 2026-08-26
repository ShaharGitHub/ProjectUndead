using UnityEngine;

public interface IWeaponLogic
{
    public BaseWeaponData GetData();
    public void SetDestroyOnEquip(bool stat);
    public void OnReleaseTrigger();
    public WeaponManager Equip(WeaponManager equipWeapon, GameObject weaponPrefab, Transform handSlot, Camera eyesCamera);
    public void Use(WeaponManager weapon);
    public void Drop(WeaponManager weapon);
}
