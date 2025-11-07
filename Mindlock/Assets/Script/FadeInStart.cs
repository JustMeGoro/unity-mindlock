using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class FadeInStart : MonoBehaviour
{
    public static FadeInStart Instance;

    [Header("Object selection")]
    public CanvasGroup wakingUpPanel;

    [Header("Settings")]
    public float soundFadeDuration = 1.5f; // Jak dlouho potrvá zesílení zvuku
    public float screenFadeDuration = 2f;  // Jak dlouho potrvá zmizení obrazovky
    public float soundStartPercent = 0.3f; // Na kolik procent zvuku se má dostat pøed rozsvícením (0.0 - 1.0)
    public float delaySeconds = 2f;      // Zpoždìní pøed zaèátkem efektu
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
        if (!skipFadeOut)
        {
            // Uložení pùvodní hlasitosti (defaultnì 1.0)
            targetVolume = PlayerPrefs.GetFloat("SavedVolume", 1f);

            // Zaèínáme potichu a s èernou obrazovkou
            AudioListener.volume = 0f;
            wakingUpPanel.alpha = 1f;


            StartCoroutine(FadeOutRoutine(delaySeconds));
        }
        else
        {
            wakingUpPanel.alpha = 0f;
            fadingOut = false;
        }
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

        // *pauza pøed rozsvícením (volitelné)*
        // yield return new WaitForSeconds(0.3f);

        // ===== FÁZE 2: MIZENÍ ÈERNÉ OBRAZOVKY + DODATEÈNÉ ZVÝŠENÍ VOLUME =====
        time = 0f;
        while (time < screenFadeDuration)
        {
            float t = time / screenFadeDuration;

            // Fade obrazovky
            wakingUpPanel.alpha = 1f - t;

            // Dovytažení hlasitosti na finální úroveò
            AudioListener.volume = Mathf.Lerp(targetVolumePhase, targetVolume, t);

            time += Time.deltaTime;
            yield return null;
        }

        wakingUpPanel.alpha = 0f;
        AudioListener.volume = targetVolume;
        fadingOut = false;
    }

}
