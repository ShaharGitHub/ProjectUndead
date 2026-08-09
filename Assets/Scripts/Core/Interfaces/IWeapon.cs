using UnityEngine;

public interface IWeapon
{
    public BaseWeaponData GetData();
    public void Pickup();
    public void Use();
}
