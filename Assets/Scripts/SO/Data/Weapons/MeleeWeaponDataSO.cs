using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponDataSO", menuName = "Weapons/Data SO/Create melee weapon data")]
public class MeleeWeaponDataSO : BaseWeaponData
{
    [Header("Stats:")]
    public float Range = 1.5f;
    public float AttackRate = 1f;
    public float HitboxRadius = 0.5f;

    [Header("Animations:")]
    public AnimationClip UseAnimation;     // <- Shoot animation

    public override IWeaponLogic CreateWeapon()
    {
        return new MeleeWeaponLogic(this);
    }
}
