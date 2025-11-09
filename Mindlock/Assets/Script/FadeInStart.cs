using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class FadeInStart : MonoBehaviour
{
    public static FadeInStart Instance;

    public Room1ObjectiveUI room1ObjectiveUI;

    [Header("Object selection")]
    public CanvasGroup wakingUpPanel;

    [Header("Settings")]
    public float soundFadeDuration = 1.5f; // Jak dlouho potrvá zesílení zvuku
    public float screenFadeDuration = 2f;  // Jak dlouho potrvá zmizení obrazovky
    public float soundStartPercent = 0.3f; // Na kolik procent zvuku se má dostat před rozsvícením (0.0 - 1.0)
    public float delaySeconds = 2f;      // Zpoždění před začátkem efektu
    public bool skipFadeOut = false;

    [Header("Helping bool")]
    public bool fadingOut = true;

    private float targetVolume;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Uložení původní hlasitosti (defaultně 1.0)
        targetVolume = PlayerPrefs.GetFloat("SavedVolume", 1f);

        // Začínáme potichu a s černou obrazovkou
        AudioListener.volume = 0f;
        wakingUpPanel.alpha = 1f;


        StartCoroutine(FadeOutRoutine(delaySeconds));
    }

    IEnumerator FadeOutRoutine(float DelaySeconds)
    {
        yield return new WaitForSeconds(DelaySeconds);

        // ===== FÁZE 1: ZESÍLENÍ ZVUKU =====
        float time = 0f;
        float targetVolumePhase = targetVolume * soundStartPercent;

        while (time < soundFadeDuration)
        {
            float t = time / soundFadeDuration;
            AudioListener.volume = Mathf.Lerp(0f, targetVolumePhase, t);
            time += Time.deltaTime;
            yield return null;
        }
        AudioListener.volume = targetVolumePhase;

        // *pauza před rozsvícením (volitelné)*
        // yield return new WaitForSeconds(0.3f);

        // ===== FÁZE 2: MIZENÍ ČERNÉ OBRAZOVKY + DODATEČNÉ ZVÝŠENÍ VOLUME =====
        time = 0f;
        while (time < screenFadeDuration)
        {
            float t = time / screenFadeDuration;

            // Fade obrazovky
            wakingUpPanel.alpha = 1f - t;

            // Dovytažení hlasitosti na finální úroveň
            AudioListener.volume = Mathf.Lerp(targetVolumePhase, targetVolume, t);

            time += Time.deltaTime;
            yield return null;
        }

        wakingUpPanel.alpha = 0f;
        AudioListener.volume = targetVolume;
        fadingOut = false;
    }

    public IEnumerator FadeToBlack(float DelaySeconds)
    {
        yield return new WaitForSeconds(DelaySeconds);

        // ===== FÁZE 1: ZTLUMENÍ ZVUKU (0 → targetVolume * soundStartPercent půjde naopak) =====
        float time;
        float startVolume = AudioListener.volume;

        // ===== FÁZE 2: ZATMAVENÍ OBRAZOVKY + DOZTIŠENÍ =====
        time = 0f;
        float screenStartAlpha = wakingUpPanel.alpha; // obvykle 0f
        float screenEndAlpha = 1f; // úplně černá

        while (time < screenFadeDuration)
        {
            float t = time / screenFadeDuration;

            // Fade obrazovky do černa
            wakingUpPanel.alpha = Mathf.Lerp(screenStartAlpha, screenEndAlpha, t);
            AudioListener.volume = Mathf.Lerp(startVolume, 0f, t);


            time += Time.deltaTime;
            yield return null;
        }

        wakingUpPanel.alpha = 1f;
        AudioListener.volume = 0f;
        fadingOut = false;

        yield return new WaitForSeconds(DelaySeconds);
        room1ObjectiveUI.SceneRestart();
    }

}
