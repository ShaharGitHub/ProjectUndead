using UnityEngine;

public class RangeWeaponLogic : IWeapon
{
    public readonly RangeWeaponDataSO _data;

    public BaseWeaponData GetData()
    {
        return _data;
    }

    public RangeWeaponLogic(RangeWeaponDataSO data)
    {
        // Set the current SO (from RangeWeaponDataSO)
        _data = data;
    }

    public void Pickup()
    {
        Debug.Log("Range weapon picked");
    }

    public void Use()
    {
        Debug.Log("Range weapon fire");

        // To use coroutine, use player manager (need monoBehaviour)
        //carManager.StartCoroutine(BoostProcess(carManager));
    }

    public void Drop()
    {
        Debug.Log("Range Weapon droped");
    }
}
