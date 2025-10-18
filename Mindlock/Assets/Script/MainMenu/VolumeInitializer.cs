using UnityEngine;

public class VolumeInitializer : MonoBehaviour
{
    private const string VolumeKey = "musicVolume";

    void Awake()
    {
        AudioListener.volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
    }
}
