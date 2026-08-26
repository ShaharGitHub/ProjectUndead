using UnityEngine;

[System.Serializable]
public class EffectsEngine
{
    [SerializeField] private EffectsFactorySO m_effectsFactorySO;

    public GameObject SpawnEffect(VfxTypes type, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
    {
        if (m_effectsFactorySO == null)
        {
            return null;
        }

        return m_effectsFactorySO.CreateEffect(type, position, rotation, parent);
    }
}
