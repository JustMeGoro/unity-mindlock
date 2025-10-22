using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VolumeInitializer : MonoBehaviour
{
    private const string MasterKey = "masterVolume";
    private const string MusicKey = "musicVolume";
    private const string SfxKey = "sfxVolume";

    // Spustí se ještě před načtením první scény -> nastaví master hned
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitAtStartup()
    {
        float master = PlayerPrefs.GetFloat(MasterKey, 1f);
        AudioListener.volume = master;

        float music = PlayerPrefs.GetFloat(MusicKey, 1f);
        ApplyMusicVolume(music);

        float sfx = PlayerPrefs.GetFloat(SfxKey, 1f);
        ApplySfxVolume(sfx);

        // reapply po jednom snímku pro AudioSources vytvořené v Start()
        var helperGO = new GameObject("VolumeInitHelper");
        Object.DontDestroyOnLoad(helperGO);
        helperGO.AddComponent<VolumeReapplyHelper>().StartReapply(music, true);
        helperGO.AddComponent<VolumeReapplyHelper>().StartReapply(sfx, false);

        SceneManager.sceneLoaded += OnSceneLoadedStatic;
    }

    private static void OnSceneLoadedStatic(Scene scene, LoadSceneMode mode)
    {
        float music = PlayerPrefs.GetFloat(MusicKey, 1f);
        ApplyMusicVolume(music);

        float sfx = PlayerPrefs.GetFloat(SfxKey, 1f);
        ApplySfxVolume(sfx);

        var helperGO = new GameObject("VolumeInitHelper");
        Object.DontDestroyOnLoad(helperGO);
        helperGO.AddComponent<VolumeReapplyHelper>().StartReapply(music, true);
        helperGO.AddComponent<VolumeReapplyHelper>().StartReapply(sfx, false);
    }

    // Pokud má tento skript připojený na GameObject, necháme i Awake fallback
    void Awake()
    {
        float master = PlayerPrefs.GetFloat(MasterKey, 1f);
        AudioListener.volume = master;

        float music = PlayerPrefs.GetFloat(MusicKey, 1f);
        ApplyMusicVolume(music);
        StartCoroutine(ReapplyNextFrame(music));

        float sfx = PlayerPrefs.GetFloat(SfxKey, 1f);
        ApplySfxVolume(sfx);
        StartCoroutine(ReapplySfxNextFrame(sfx));
    }

    private IEnumerator ReapplyNextFrame(float music)
    {
        yield return null;
        ApplyMusicVolume(music);
    }

    private IEnumerator ReapplySfxNextFrame(float sfx)
    {
        yield return null;
        ApplySfxVolume(sfx);
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

    // Pomocník pro reapply po jednom frame (static initializer nemůže coroutine přímo)
    private class VolumeReapplyHelper : MonoBehaviour
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
