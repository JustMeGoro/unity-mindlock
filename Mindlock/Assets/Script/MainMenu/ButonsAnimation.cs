using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButonsAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Tooltip("Na kolik se zmenší text pøi hover (1 = bez zmìny, 0.9 = 90%).")]
    public float scaleFactor = 0.9f;

    [Tooltip("Multiplikátor jasu barvy textu pøi hover (1 = bez zmìny, 0.6 = ztmavení).")]
    [Range(0f, 1f)]
    public float darkenMultiplier = 0.6f;

    TMP_Text tmpText;
    Text uiText;
    Transform textTransform;

    Color originalColor;
    Vector3 originalScale;

    void Awake()
    {
        tmpText = GetComponentInChildren<TMP_Text>(true);
        if (tmpText != null)
        {
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
                Debug.LogWarning($"ButonsAnimation: žádný TMP_Text ani UI.Text nenalezen u '{name}'.", this);
            }
        }

        if (textTransform != null)
            originalScale = textTransform.localScale;
    }

    // IPointer
    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ApplyHover(false);
    }

    // Selection (keyboard/joystick)
    public void OnSelect(BaseEventData eventData)
    {
        ApplyHover(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ApplyHover(false);
    }

    void ApplyHover(bool hover)
    {
        if (textTransform == null) return;

        // scale
        textTransform.localScale = hover ? originalScale * scaleFactor : originalScale;

        // color
        if (tmpText != null)
        {
            if (hover)
                tmpText.color = new Color(originalColor.r * darkenMultiplier, originalColor.g * darkenMultiplier, originalColor.b * darkenMultiplier, originalColor.a);
            else
                tmpText.color = originalColor;
        }
        else if (uiText != null)
        {
            if (hover)
                uiText.color = new Color(originalColor.r * darkenMultiplier, originalColor.g * darkenMultiplier, originalColor.b * darkenMultiplier, originalColor.a);
            else
                uiText.color = originalColor;
        }
    }

    void OnDisable()
    {
        // vždy resetuj, když se objekt deaktivuje nebo panel zavøe
        ApplyHover(false);
    }

    void OnEnable()
    {
        // pokud je momentálnì vybraný, zobraz hover stav; jinak resetuj
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            ApplyHover(true);
        else
            ApplyHover(false);
    }

    // veøejná metoda pro manuální reset z jiného skriptu (napø. pøi zavøení Options)
    public void ResetVisuals()
    {
        ApplyHover(false);
    }
}

