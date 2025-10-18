using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    private const string VolumeKey = "musicVolume";

    void Awake()
    {
        // 🔊 Aplikuj hlasitost hned po spuštění hry (ještě před aktivací menu)
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        AudioListener.volume = savedVolume;
    }

    void Start()
    {
        // Teď načti do slideru, když se menu otevře
        volumeSlider.value = PlayerPrefs.GetFloat(VolumeKey, 1f);
        volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(); });
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat(VolumeKey, volumeSlider.value);
        PlayerPrefs.Save();
    }
}