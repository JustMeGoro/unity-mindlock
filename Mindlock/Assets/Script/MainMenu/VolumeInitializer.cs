using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VolumeInitializer : MonoBehaviour
{
    private const string MasterKey = "masterVolume";
    private const string MusicKey = "musicVolume";

    // Spust� se je�t� p�ed na�ten�m prvn� sc�ny -> nastav� master hned
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitAtStartup()
    {
        float master = PlayerPrefs.GetFloat(MasterKey, 1f);
        AudioListener.volume = master;

        float music = PlayerPrefs.GetFloat(MusicKey, 1f);
        ApplyMusicVolume(music);

        // reapply po jednom sn�mku pro AudioSources vytvo�en� v Start()
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

    // Pokud m� tento skript p�ipojen� na GameObject, nech�me i Awake fallback
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

    // Aplikuje hlasitost pouze na zdroje ozna�en� tagem "Music"
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
            // tag nemus� b�t definovan� -> nic neaplikujeme
            return;
        }

        foreach (var go in musicObjects)
        {
            var src = go.GetComponent<AudioSource>();
            if (src != null)
                src.volume = linear;
        }
    }

    // Pomocn�k pro reapply po jednom frame (static initializer nem��e coroutine p��mo)
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
