using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectsFactorySO", menuName = "Factories/Effects/Create effects factory")]
public class EffectsFactorySO : ScriptableObject
{
    [System.Serializable]
    public class EffectData
    {
        public VfxTypes Type;
        public GameObject Effect;
        public float Duration;
    }

    public EffectData[] Data;

    private Dictionary<VfxTypes, EffectData> m_effectsDict;


    private void OnEnable()
    {
        BuildEffectsDict();
    }

    private void BuildEffectsDict()
    {
        if (Data == null) return;

        // Create new dictionary
        m_effectsDict = new Dictionary<VfxTypes, EffectData>();

        // Run over the dictionary
        foreach (EffectData data in Data)
        {
            // If effect not exist, add it
            if (!m_effectsDict.ContainsKey(data.Type))
                m_effectsDict.Add(data.Type, data);
            
            // If effect exist, ignore
            else
                Debug.LogWarning($"EffectsFactorySO: Duplicate effect name '{data.Type}' - ignoring duplicate.");
        }
    }

    // Vector3? -> turning the variable to be able to have Null.
    public GameObject CreateEffect(VfxTypes type, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
    {
        // Create effects dictionary if not exist
        if (m_effectsDict == null) BuildEffectsDict();

        // Try to get current effect by name
        if (!m_effectsDict.TryGetValue(type, out EffectData data))
        {
            Debug.LogError($"EffectsFactorySO: Effect '{type}' not found!");
            return null;
        }

        // Set default value
        Vector3 pos = position == null ? Vector3.zero : position.Value;
        Quaternion rot = rotation == null ? Quaternion.identity : rotation.Value;

        // Option 2: Set default value
        //Vector3 pos = position ?? Vector3.zero;
        //Quaternion rot = rotation ?? Quaternion.identity;

        // Create the effect
        GameObject effect = Instantiate(data.Effect, pos, rot, parent);

        // Reset position and rotation if having parent
        if (parent != null && position == null)
        {
            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.identity;
        }

        // Destroy the effect after X time
        float duration = GetEffectDuration(effect, data.Duration);
        if (duration > 0f)
        {
            Destroy(effect, duration);
        }



        return effect;
    }

    private float GetEffectDuration(GameObject effect, float manualOverride)
    {
        // Return manual duration from inspector (if have)
        if (manualOverride > 0f)
            return manualOverride;

        // Get all particle systems from the object
        ParticleSystem[] allSystems = effect.GetComponentsInChildren<ParticleSystem>();
        if (allSystems.Length > 0)
        {
            // Check for the longest duration effect
            float maxDuration = 0f;
            foreach (ParticleSystem ps in allSystems)
            {
                // Check for combine duration (duration + start life time of particles)
                float total = ps.main.duration + ps.main.startLifetime.constantMax;
                maxDuration = Mathf.Max(maxDuration, total);
            }
            return maxDuration;
        }

        // Object have no particle system and no manual duration
        Debug.LogWarning($"EffectsFactorySO: '{effect.name}' has no manual Duration and no ParticleSystem - won't auto-destroy.");
        return -1f;
    }
}
