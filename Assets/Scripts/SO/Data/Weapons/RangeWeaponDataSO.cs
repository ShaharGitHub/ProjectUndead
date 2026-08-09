using UnityEngine;

public enum WeaponSubTypes { Pistol, AR, SMG, MG, Shotgun, Sniper, Launcher }

[CreateAssetMenu(fileName = "RangeWeaponDataSO", menuName = "Weapons/Data SO/Create range weapon data")]
public class RangeWeaponDataSO : BaseWeaponData
{
    public WeaponSubTypes WeaponSubType;

    [Header("Ammo:")]
    public int MaxAmmo;
    public int ClipSize;

    [Header("Stats:")]
    public float FireRate;
    public float Accuracy;
    public float Range;
    public float ReloadTime;
    public float Spread;                   // <- For shotguns

    [Header("Animations:")]
    public AnimationClip UseAnimation;     // <- Shoot animation
    public AnimationClip ReloadAnimation;  // <- Reload animation

    public override IWeapon CreateWeapon()
    {
        return new RangeWeaponLogic(this);
    }
}
