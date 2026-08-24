using UnityEngine;

[CreateAssetMenu(fileName = "AudioFactorySO", menuName = "Factories/Audio/Create audio factory")]
public class AudioFactorySO : ScriptableObject
{
    [System.Serializable]
    public class AudioData
    {
        public string Name;
        public AudioClip Clip;
    }

    public AudioData[] Data;


    public AudioClip GetClipByName(string name)
    {
        foreach (AudioData data in Data)
        {
            if (data.Name == name)
                return data.Clip;
        }
        return null;
    }
}
