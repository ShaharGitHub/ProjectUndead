using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponDataSO", menuName = "Weapons/Data SO/Create melee weapon data")]
public class MeleeWeaponDataSO : BaseWeaponData
{
    [Header("Stats:")]
    public float DelayTime;                // <- Should be use by animation clip length

    [Header("Animations:")]
    public AnimationClip UseAnimation;     // <- Shoot animation
}
