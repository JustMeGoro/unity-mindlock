using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider; // master
    [SerializeField] private Slider musicSlider;  // hudba
    [SerializeField] private Slider sfxSlider;    // efekty (SFX)

    public const string VolumeKey = "masterVolume";
    public const string MusicKey = "musicVolume";
    public const string SfxKey = "sfxVolume";

    // Spustí se ještě před načtením první scény -> zajistí master volume hned při startu
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeAudioAtStartup()
    {
        float savedMaster = PlayerPrefs.GetFloat(VolumeKey, 1f);
        AudioListener.volume = savedMaster;

        // Zajistí aplikaci hlasitosti hudby a SFX po načtení každé scény
        SceneManager.sceneLoaded += OnSceneLoadedStatic;
    }

    private static void OnSceneLoadedStatic(Scene scene, LoadSceneMode mode)
    {
        float savedMusic = PlayerPrefs.GetFloat(MusicKey, 1f);
        ApplyMusicVolume(savedMusic);

        float savedSfx = PlayerPrefs.GetFloat(SfxKey, 1f);
        ApplySfxVolume(savedSfx);

        // Pomocník který znovu aplikuje po jednom frame (AudioSources mohou být vytvořeny v Start())
        var helperGO = new GameObject("AudioReapplyHelper");
        Object.DontDestroyOnLoad(helperGO);
        helperGO.AddComponent<AudioReapplyHelper>().StartReapply(savedMusic, true);
        helperGO.AddComponent<AudioReapplyHelper>().StartReapply(savedSfx, false);
    }

    void Awake()
    {
        // Stále bezpečně aplikujeme i zde (duplicitní, ale bez škod)
        float savedMaster = PlayerPrefs.GetFloat(VolumeKey, 1f);
        AudioListener.volume = savedMaster;

        float savedMusic = PlayerPrefs.GetFloat(MusicKey, 1f);
        ApplyMusicVolume(savedMusic);
        StartCoroutine(ReapplyMusicNextFrame(savedMusic));

        float savedSfx = PlayerPrefs.GetFloat(SfxKey, 1f);
        ApplySfxVolume(savedSfx);
        StartCoroutine(ReapplySfxNextFrame(savedSfx));

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Inicializace sliderů bez vyvolání onValueChanged
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(VolumeKey, 1f));
            volumeSlider.onValueChanged.AddListener((v) => ChangeVolume(v));
        }

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(MusicKey, 1f));
            musicSlider.onValueChanged.AddListener((v) => ChangeMusicVolume(v));
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(SfxKey, 1f));
            sfxSlider.onValueChanged.AddListener((v) => ChangeSfxVolume(v));
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // Pozor: statická registrace z InitializeAudioAtStartup zůstane aktivní po dobu běhu aplikace.
        // Pokud chcete uvolnit i tuto, odkomentujte následující řádek:
        // SceneManager.sceneLoaded -= OnSceneLoadedStatic;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        float savedMusic = PlayerPrefs.GetFloat(MusicKey, 1f);
        ApplyMusicVolume(savedMusic);
        StartCoroutine(ReapplyMusicNextFrame(savedMusic));

        float savedSfx = PlayerPrefs.GetFloat(SfxKey, 1f);
        ApplySfxVolume(savedSfx);
        StartCoroutine(ReapplySfxNextFrame(savedSfx));
    }

    private IEnumerator ReapplyMusicNextFrame(float volume)
    {
        yield return null; // počkej jeden frame, aby se stihly provést Start() ostatních objektů
        ApplyMusicVolume(volume);
    }

    private IEnumerator ReapplySfxNextFrame(float volume)
    {
        yield return null; // počkej jeden frame, aby se stihly provést Start() ostatních objektů
        ApplySfxVolume(volume);
    }

    // Volá se z onValueChanged (param float)
    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    // Volá se z onValueChanged (param float)
    public void ChangeMusicVolume(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(MusicKey, value);
        PlayerPrefs.Save();
    }

    // Volá se z onValueChanged (param float)
    public void ChangeSfxVolume(float value)
    {
        ApplySfxVolume(value);
        PlayerPrefs.SetFloat(SfxKey, value);
        PlayerPrefs.Save();
    }

    // Legacy / Inspektor: pokud máš v Inspektoru nastavené volání bez parametru
    public void ChangeVolume()
    {
        if (volumeSlider == null) return;
        ChangeVolume(volumeSlider.value);
    }

    public void ChangeMusicVolume()
    {
        if (musicSlider == null) return;
        ChangeMusicVolume(musicSlider.value);
    }

    public void ChangeSfxVolume()
    {
        if (sfxSlider == null) return;
        ChangeSfxVolume(sfxSlider.value);
    }

    // Aplikuje hlasitost pouze na zdroje označené tagem "Music"
    private static void ApplyMusicVolume(float linear)
    {
        linear = Mathf.Clamp01(linear);

        GameObject[] musicObjects;
        try
        {
            musicObjects = GameObject.FindGameObjectsWithTag("Music");
        }
        catch
        {
            // tag nemusí být definovaný -> nic neaplikujeme
            return;
        }

        foreach (var go in musicObjects)
        {
            var src = go.GetComponent<AudioSource>();
            if (src != null)
                src.volume = linear;
        }
    }

    // Aplikuje hlasitost pouze na zdroje označené tagem "SFX"
    private static void ApplySfxVolume(float linear)
    {
        linear = Mathf.Clamp01(linear);

        GameObject[] sfxObjects;
        try
        {
            sfxObjects = GameObject.FindGameObjectsWithTag("SFX");
        }
        catch
        {
            // tag nemusí být definovaný -> nic neaplikujeme
            return;
        }

        foreach (var go in sfxObjects)
        {
            var src = go.GetComponent<AudioSource>();
            if (src != null)
                src.volume = linear;
        }
    }

    // Dočasný pomocník pro reapply po jednom frame
    private class AudioReapplyHelper : MonoBehaviour
    {
        private float vol;
        private bool isMusic;

        // isMusic == true -> aplikuje Music, false -> aplikuje SFX
        public void StartReapply(float volume, bool applyMusic)
        {
            vol = volume;
            isMusic = applyMusic;
            StartCoroutine(Reapply());
        }

        private IEnumerator Reapply()
        {
            yield return null;
            if (isMusic)
                ApplyMusicVolume(vol);
            else
                ApplySfxVolume(vol);
            Destroy(gameObject);
        }
    }
}