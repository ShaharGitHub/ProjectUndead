using UnityEngine;

public class ThrowableWeaponLogic : IWeapon
{
    public readonly ThrowableWeaponDataSO _data;

    public BaseWeaponData GetData()
    {
        return _data;
    }

    public ThrowableWeaponLogic(ThrowableWeaponDataSO data)
    {
        // Set the current SO (from ThrowableWeaponDataSO)
        _data = data;
    }

    public void Pickup()
    {
        Debug.Log("Throwable weapon picked");
    }

    public void Use()
    {
        Debug.Log("Throwable weapon expload");

        // To use coroutine, use player manager (need monoBehaviour)
        //carManager.StartCoroutine(BoostProcess(carManager));
    }
}
