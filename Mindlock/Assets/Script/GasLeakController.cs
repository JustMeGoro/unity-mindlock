using System.Collections;
using UnityEngine;

public class GasLeakController : MonoBehaviour
{
    public static GasLeakController Instance;

    public ParticleSystem gasEffect;     // Tvùj particle efekt plynu
    public AudioSource gasSound;         // Zvuk plynu (pokud máš)
    public bool isLeaking = true;        // Stav úniku

    public float fadeOutDuration = 1.0f; // fade out effect


    void Awake()
    {
        Instance = this;
    }

    public void StopGas()
    {
        if (isLeaking)
        {
            isLeaking = false;

            if (gasEffect != null)
                gasEffect.Stop();

            if (gasSound != null)
                StartCoroutine(FadeOutSound());

            Debug.Log("Gas leak stopped.");
        }
    }

    IEnumerator FadeOutSound()
    {
        float startVolume = gasSound.volume;

        for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
        {
            gasSound.volume = Mathf.Lerp(startVolume, 0, t / fadeOutDuration);
            yield return null;
        }

        gasSound.volume = 0;
        gasSound.Stop();
        gasSound.volume = startVolume; // reset pro pøíští použití
    }
}
