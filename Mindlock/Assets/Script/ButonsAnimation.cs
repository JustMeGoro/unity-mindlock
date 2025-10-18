using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButonsAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Doba animace zmìny (v sekundách).")]
    public float duration = 0.12f;

    [Tooltip("Na kolik se zmenší text pøi hover (1 = bez zmìny, 0.9 = 90%).")]
    public float scaleFactor = 0.9f;

    [Tooltip("Multiplikátor jasu barvy textu pøi hover (1 = bez zmìna, 0.6 = ztmavení).")]
    [Range(0f, 1f)]
    public float darkenMultiplier = 0.6f;

    TMP_Text tmpText;
    Text uiText;
    Transform textTransform;

    Color originalColor;
    Vector3 originalScale;

    Coroutine animCoroutine;

    void Awake()
    {
        // Najde TextMeshPro nebo starý UI Text ve children
        tmpText = GetComponentInChildren<TMP_Text>(true);
        if (tmpText != null)
        {
            uiText = null;
            textTransform = tmpText.transform;
            originalColor = tmpText.color;
        }
        else
        {
            uiText = GetComponentInChildren<Text>(true);
            if (uiText != null)
            {
                textTransform = uiText.transform;
                originalColor = uiText.color;
            }
            else
            {
                Debug.LogWarning($"ButonsAnimation: žádný TMP_Text ani UI.Text nenalezen u '{name}'. Pøidej text jako child.", this);
            }
        }

        if (textTransform != null)
            originalScale = textTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textTransform == null) return;
        StartAnim(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (textTransform == null) return;
        StartAnim(false);
    }

    void StartAnim(bool hover)
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimateHover(hover));
    }

    IEnumerator AnimateHover(bool hover)
    {
        float elapsed = 0f;
        Vector3 fromScale = textTransform.localScale;
        Vector3 toScale = hover ? originalScale * scaleFactor : originalScale;

        Color fromColor = (tmpText != null) ? tmpText.color : uiText.color;
        Color targetColor = originalColor;
        if (hover)
            targetColor = new Color(originalColor.r * darkenMultiplier, originalColor.g * darkenMultiplier, originalColor.b * darkenMultiplier, originalColor.a);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            textTransform.localScale = Vector3.Lerp(fromScale, toScale, t);
            Color c = Color.Lerp(fromColor, targetColor, t);
            if (tmpText != null) tmpText.color = c;
            if (uiText != null) uiText.color = c;
            yield return null;
        }

        // ensure final state
        textTransform.localScale = toScale;
        if (tmpText != null) tmpText.color = targetColor;
        if (uiText != null) uiText.color = targetColor;

        animCoroutine = null;
    }
}

