using UnityEngine;

public class WeaponCollision : BaseWeaponService, IWeapon
{
    public void Equip()
    {
        Debug.Log("Weapon equipped");
    }

    public void Use()
    {
        Debug.Log("Weapon used");
    }

    //public void Reload()
    //{
    //    // Maybe not here - consider moving to 'Ammo' Script
    //}

    public void Drop()
    {
        Debug.Log("Weapon dropped");
    }
}
