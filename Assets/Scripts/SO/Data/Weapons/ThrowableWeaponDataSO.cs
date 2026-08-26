using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableWeaponDataSO", menuName = "Weapons/Data SO/Create throwable weapon data")]
public class ThrowableWeaponDataSO : BaseWeaponData
{
    [Header("Ammo:")]
    public int MaxAmmo;

    [Header("Stats:")]
    public bool IsDamage;
    public float Radius;
    public float TimeToExpload;

    [Header("VFX:")]
    public VfxTypes VfxType;

    [Header("Animations:")]
    public AnimationClip HoldAnimation;
    public AnimationClip ReleaseAnimation;

    public override IWeaponLogic CreateWeapon()
    {
        return new ThrowableWeaponLogic(this);
    }
}
