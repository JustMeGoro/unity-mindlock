using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectiveNotification : MonoBehaviour
{
    public static ObjectiveNotification Instance;

    public TMP_Text text;
    public CanvasGroup canvasGroup;

    [Header("Text Settings")]
    public string message = "NEW OBJECTIVE";

    [Header("Timing")]
    public float showDuration = 2f;   // Jak dlouho zùstane text na obrazovce
    public float fadeDuration = 0.4f; // Rychlost fade-in a fade-out
    public float pulseSpeed = 6f;     // Rychlost blikání barvy

    [Header("Color Pulse")]
    public Color colorA = new Color(0.25f, 0.45f, 1f); // základní modrá (uprav podle obrázku)
    public Color colorB = new Color(0.45f, 0.65f, 1f); // svìtlejší modrá variace

    void Awake()
    {
        text.text = message;
        Instance = this;
        canvasGroup.alpha = 0;
    }

    public void Show()
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        // Fade-in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Pulse barvy (bìhem trvání textu)
        float timer = 0f;
        while (timer < showDuration)
        {
            timer += Time.deltaTime;
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;  // 0–1 oscillace
            text.color = Color.Lerp(colorA, colorB, pulse);
            yield return null;
        }

        // Fade-out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}
