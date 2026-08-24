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
    [Range(0.5f, 20f)] public float FireRate;
    public float Accuracy;
    [Range(10f, 1000f)] public float Range;
    public float ReloadTime;
    public float Spread;                   // <- For shotguns

    [Header("Animations:")]
    public AnimationClip UseAnimation;     // <- Shoot animation
    public AnimationClip ReloadAnimation;  // <- Reload animation

    public override IWeaponLogic CreateWeapon()
    {
        return new RangeWeaponLogic(this);
    }
}
