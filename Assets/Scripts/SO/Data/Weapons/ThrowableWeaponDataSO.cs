using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableWeaponDataSO", menuName = "Weapons/Data SO/Create throwable weapon data")]
public class ThrowableWeaponDataSO : BaseWeaponData
{
    [Header("Ammo:")]
    public int MaxAmount;

    [Header("Stats:")]
    public float Radius;
    public float TimeToExpload;

    [Header("Animations:")]
    public AnimationClip HoldAnimation;
    public AnimationClip ReleaseAnimation;

    public override IWeaponLogic CreateWeapon()
    {
        return new ThrowableWeaponLogic(this);
    }
}
