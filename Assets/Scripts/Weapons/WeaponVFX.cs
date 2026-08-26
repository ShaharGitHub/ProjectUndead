using UnityEngine;

public class WeaponVFX : BaseWeaponService
{
    [SerializeField] private EffectsEngine m_effectsEngine;

    public void SpawnEffect(GameObject source, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
    {
        VfxTypes effect = IdentifyEffect(source);
        m_effectsEngine?.SpawnEffect(effect, position, rotation, parent);
    }

    private VfxTypes IdentifyEffect(GameObject source)
    {
        // Muzzle flash effect
        if (source.TryGetComponent<MuzzlePosition>(out _))
            return VfxTypes.MuzzleFlash;

        //// Hit enemy effect
        //if (source.TryGetComponent<IEnemy>(out _))
        //    return EffectTypes.ZombieBloodImpact;

        //// Hit player effect (for multiplayer)
        //if (source.TryGetComponent<IPlayer>(out _))
        //    return EffectTypes.PlayerHitImpact;

        // Default bullet impact effect
        return VfxTypes.BulletImpact;
    }
}
