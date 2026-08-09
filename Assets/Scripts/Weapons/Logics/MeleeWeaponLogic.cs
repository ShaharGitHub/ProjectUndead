using UnityEngine;

public class MeleeWeaponLogic : IWeapon
{
    public readonly MeleeWeaponDataSO _data;

    public BaseWeaponData GetData()
    {
        return _data;
    }

    public MeleeWeaponLogic(MeleeWeaponDataSO data)
    {
        // Set the current SO (from MeleeWeaponDataSO)
        _data = data;
    }

    public void Pickup()
    {
        Debug.Log("Melee weapon picked");
    }

    public void Use()
    {
        Debug.Log("Melee weapon slice");

        // To use coroutine, use player manager (need monoBehaviour)
        //carManager.StartCoroutine(BoostProcess(carManager));
    }
}
