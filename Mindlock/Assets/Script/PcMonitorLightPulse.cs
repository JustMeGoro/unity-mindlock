using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class SimpleMonitorPulse : MonoBehaviour
{
    public Light2D light1;
    public Light2D light2;
    public SpriteRenderer monitorSprite;

    [Header("Nastavení")]
    public float startDelay = 2f;         // čekání před startem pulzování
    public float fadeInTime = 2f;         // plynulé rozsvícení do 0.5
    public float pulseSpeed = 2f;         // rychlost pulzování
    public float minIntensity = 0.5f;     // minimální intenzita při pulzu
    public float maxIntensity = 1.5f;     // maximální intenzita při pulzu
    public float spriteFadeMultiplier = 0.6f;

    private bool pulsing = false;
    private Coroutine pulseCoroutine;
    private Coroutine fadeOutCoroutine;

    void Start()                                                
    {
        SetLightIntensity(0f);
        SetSpriteAlpha(0f);
    }

    void Update()
    {
        var controller = PlayerLightAlarmController.Instance;
        if (controller == null) return;

        bool shouldPulse = controller.alarmActive == false && controller.systemsOnline == false;

        if (shouldPulse)
        {
            if (pulseCoroutine == null)
            {
                if (fadeOutCoroutine != null)
                {
                    StopCoroutine(fadeOutCoroutine);
                    fadeOutCoroutine = null;
                }
                pulseCoroutine = StartCoroutine(BeginPulseAfterDelay());
            }
        }
        else
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
                pulsing = false;
            }

            if (fadeOutCoroutine == null)
            {
                fadeOutCoroutine = StartCoroutine(FadeOutToZero());
            }
        }
    }

    private IEnumerator BeginPulseAfterDelay()
    {
        // povolí přístup k počítači
        MonitorUI.Instance.isComputerAccesable = true;

        // počkej na start, ale kontroluj stav každým snímkem
        float waited = 0f;
        while (waited < startDelay)
        {
            var inst = PlayerLightAlarmController.Instance;
            if (inst == null || inst.alarmActive || inst.systemsOnline)
            {
                pulseCoroutine = null;
                yield break;
            }

            waited += Time.deltaTime;
            yield return null;
        }

        // fade-in
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            var inst = PlayerLightAlarmController.Instance;
            if (inst == null || inst.alarmActive || inst.systemsOnline)
            {
                pulseCoroutine = null;
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / fadeInTime;
            float newIntensity = Mathf.Lerp(0f, minIntensity, t);
            SetLightIntensity(newIntensity);
            SetSpriteAlpha(newIntensity * spriteFadeMultiplier);
            yield return null;
        }

        // zapni pulzování
        pulsing = true;
        while (pulsing)
        {
            var inst = PlayerLightAlarmController.Instance;
            if (inst == null || inst.alarmActive || inst.systemsOnline)
            {
                pulsing = false;
                break;
            }

            float pulse = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            SetLightIntensity(pulse);
            SetSpriteAlpha(pulse * spriteFadeMultiplier);
            yield return null;
        }

        pulseCoroutine = null;
    }

    private IEnumerator FadeOutToZero()
    {
        float elapsed = 0f;
        float startIntensity = GetCurrentIntensity();
        float startAlpha = monitorSprite != null ? monitorSprite.color.a : 0f;

        if (Mathf.Approximately(startIntensity, 0f) && Mathf.Approximately(startAlpha, 0f))
        {
            fadeOutCoroutine = null;
            yield break;
        }

        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInTime;
            float newIntensity = Mathf.Lerp(startIntensity, 0f, t);
            float newAlpha = Mathf.Lerp(startAlpha, 0f, t);
            SetLightIntensity(newIntensity);
            SetSpriteAlpha(newAlpha);
            yield return null;
        }

        SetLightIntensity(0f);
        SetSpriteAlpha(0f);
        fadeOutCoroutine = null;
    }

    private float GetCurrentIntensity()
    {
        if (light1 != null) return light1.intensity;
        if (light2 != null) return light2.intensity;
        return 0f;
    }

    private void SetLightIntensity(float value)
    {
        if (light1 != null) light1.intensity = value;
        if (light2 != null) light2.intensity = value;
    }

    private void SetSpriteAlpha(float value)
    {
        if (monitorSprite == null) return;

        Color c = monitorSprite.color;
        c.a = Mathf.Clamp01(value);
        monitorSprite.color = c;
    }
}
