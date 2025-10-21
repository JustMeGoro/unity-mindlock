using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VolumeInitializer : MonoBehaviour
{
    private const string MasterKey = "masterVolume";
    private const string MusicKey = "musicVolume";

    // Spustí se ještì pøed naètením první scény -> nastaví master hned
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitAtStartup()
    {
        float master = PlayerPrefs.GetFloat(MasterKey, 1f);
        AudioListener.volume = master;

        float music = PlayerPrefs.GetFloat(MusicKey, 1f);
        ApplyMusicVolume(music);

        // reapply po jednom snímku pro AudioSources vytvoøené v Start()
        var helperGO = new GameObject("VolumeInitHelper");
        Object.DontDestroyOnLoad(helperGO);
        helperGO.AddComponent<VolumeReapplyHelper>().StartReapply(music);

        SceneManager.sceneLoaded += OnSceneLoadedStatic;
    }

    private static void OnSceneLoadedStatic(Scene scene, LoadSceneMode mode)
    {
        float music = PlayerPrefs.GetFloat(MusicKey, 1f);
        ApplyMusicVolume(music);

        var helperGO = new GameObject("VolumeInitHelper");
        Object.DontDestroyOnLoad(helperGO);
        helperGO.AddComponent<VolumeReapplyHelper>().StartReapply(music);
    }

    // Pokud máš tento skript pøipojený na GameObject, necháme i Awake fallback
    void Awake()
    {
        float master = PlayerPrefs.GetFloat(MasterKey, 1f);
        AudioListener.volume = master;

        float music = PlayerPrefs.GetFloat(MusicKey, 1f);
        ApplyMusicVolume(music);
        StartCoroutine(ReapplyNextFrame(music));
    }

    private IEnumerator ReapplyNextFrame(float music)
    {
        yield return null;
        ApplyMusicVolume(music);
    }

    // Aplikuje hlasitost pouze na zdroje oznaèené tagem "Music"
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

    // Pomocník pro reapply po jednom frame (static initializer nemùže coroutine pøímo)
    private class VolumeReapplyHelper : MonoBehaviour
    {
        private float vol;
        public void StartReapply(float volume)
        {
            vol = volume;
            StartCoroutine(Reapply());
        }

        private IEnumerator Reapply()
        {
            yield return null;
            ApplyMusicVolume(vol);
            Destroy(gameObject);
        }
    }
}
