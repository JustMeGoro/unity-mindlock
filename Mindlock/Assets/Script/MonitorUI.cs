using System.Reflection;
using UnityEngine;

public class MonitorUI : MonoBehaviour
{
    public static MonitorUI Instance;

    [Header("References")]
    public GameObject monitorUI;                 // UI panel / prefab (Canvas or world object)
    public MonoBehaviour movementComponent;      // pøiøaï movement komponentu v Inspectoru (preferované)

    [Header("Animation (UI)")]
    public RectTransform animatedRect;           // pokud monitorUI je UI element (Canvas), pøiøaï jeho RectTransform
    public Transform animatedTransform;          // fallback - world transform animace (pokud není UI)
    public float moveDuration = 0.25f;           // délka pøiletìní (pohyb)
    public float zoomDuration = 0.25f;           // délka pøiblížení (zoom)
    public float pauseBetween = 0.15f;           // krátká pauza mezi pohybem a zoomem
    public float startYOffset = -800f;           // offset zespodu (v jednotkách RectTransformu) - negative = pod obrazovkou
    public float startScale = 0.5f;              // poèáteèní scale pøi pøiletìní
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1); // easing

    [Header("Interaction")]
    public bool playerInRange = false;
    public bool isComputerAccesable = true;      // pokud je false, hráè nemùže interagovat (kontrolováno v triggeru)

    // interní
    private GameObject currentPlayer;
    private Behaviour blockedMovementComponent;
    private bool monitorOpen = false;
    private Vector2 rectTargetAnchoredPos;
    private Vector3 transformTargetLocalPos;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        Instance = this;
        if (monitorUI != null) monitorUI.SetActive(false);
        if (animatedRect != null)
            rectTargetAnchoredPos = animatedRect.anchoredPosition;
        if (animatedTransform != null)
            transformTargetLocalPos = animatedTransform.localPosition;
    }

    void Update()
    {
        // pokud je hráè v dosahu a stiskne E -> toggle monitor a zablokuj movement
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && isComputerAccesable)
        {
            ToggleMonitor();
        }

        // zavøi monitor ESC
        if (monitorOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMonitor();
        }
    }

    public void ToggleMonitor()
    {
        if (monitorOpen)
            StartClose();
        else
            StartOpen();
    }

    private void StartOpen()
    {
        // najdi hráèe (pokud není nalezen pøes trigger)
        if (currentPlayer == null)
            currentPlayer = GameObject.FindGameObjectWithTag("Player");

        if (currentPlayer != null)
        {
            // pokud je explicitnì pøiøazena movementComponent v Inspectoru, použij ji
            if (movementComponent != null)
            {
                blockedMovementComponent = movementComponent;
            }
            else
            {
                blockedMovementComponent = FindLikelyMovementComponent(currentPlayer);
            }

            if (blockedMovementComponent != null)
            {
                // pokud komponenta má bool pole/property "canMove", nastav ji na false, jinak disable component
                if (!TrySetBoolFieldOrProperty(blockedMovementComponent, "canMove", false))
                    blockedMovementComponent.enabled = false;
            }
        }
        else
        {
            Debug.LogWarning("[MonitorUI] Player nenalezen (tag 'Player').");
        }

        // start animation coroutine
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(OpenSequence());
    }

    private void StartClose()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(CloseSequence());
    }

    private System.Collections.IEnumerator OpenSequence()
    {
        monitorOpen = true;

        if (monitorUI != null) monitorUI.SetActive(true);

        // UI element animation: nejprve posun, krátká pauza, pak zoom
        if (animatedRect != null)
        {
            rectTargetAnchoredPos = rectTargetAnchoredPos == Vector2.zero ? animatedRect.anchoredPosition : rectTargetAnchoredPos;

            Vector2 startPos = rectTargetAnchoredPos + new Vector2(0f, startYOffset);
            Vector2 endPos = rectTargetAnchoredPos;
            animatedRect.anchoredPosition = startPos;
            animatedRect.localScale = Vector3.one * startScale;

            // 1) pohyb (pøiletìní)
            float t = 0f;
            while (t < moveDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, moveDuration));
                float e = ease.Evaluate(k);
                animatedRect.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, e);
                yield return null;
            }
            animatedRect.anchoredPosition = endPos;

            // krátká pauza
            if (pauseBetween > 0f)
                yield return new WaitForSecondsRealtime(pauseBetween);

            // 2) zoom (pøiblížení)
            t = 0f;
            Vector3 sStart = Vector3.one * startScale;
            Vector3 sEnd = Vector3.one;
            while (t < zoomDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, zoomDuration));
                float e = ease.Evaluate(k);
                animatedRect.localScale = Vector3.LerpUnclamped(sStart, sEnd, e);
                yield return null;
            }
            animatedRect.localScale = sEnd;
        }
        else if (animatedTransform != null)
        {
            transformTargetLocalPos = transformTargetLocalPos == Vector3.zero ? animatedTransform.localPosition : transformTargetLocalPos;

            Vector3 startPos = transformTargetLocalPos + new Vector3(0f, startYOffset * 0.01f, 0f);
            Vector3 endPos = transformTargetLocalPos;
            animatedTransform.localPosition = startPos;
            animatedTransform.localScale = Vector3.one * startScale;

            // 1) pohyb
            float t = 0f;
            while (t < moveDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, moveDuration));
                float e = ease.Evaluate(k);
                animatedTransform.localPosition = Vector3.LerpUnclamped(startPos, endPos, e);
                yield return null;
            }
            animatedTransform.localPosition = endPos;

            // pauza
            if (pauseBetween > 0f)
                yield return new WaitForSecondsRealtime(pauseBetween);

            // 2) zoom
            t = 0f;
            Vector3 sStart = Vector3.one * startScale;
            Vector3 sEnd = Vector3.one;
            while (t < zoomDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, zoomDuration));
                float e = ease.Evaluate(k);
                animatedTransform.localScale = Vector3.LerpUnclamped(sStart, sEnd, e);
                yield return null;
            }
            animatedTransform.localScale = sEnd;
        }

        animationCoroutine = null;
        UIManager.Instance?.HideText();
    }

    private System.Collections.IEnumerator CloseSequence()
    {
        // Close: nejprve zoom out, pauza, pak odletìt dolù
        if (animatedRect != null)
        {
            Vector2 startPos = animatedRect.anchoredPosition;
            Vector2 endPos = rectTargetAnchoredPos + new Vector2(0f, startYOffset);
            Vector3 sStart = animatedRect.localScale;
            Vector3 sEnd = Vector3.one * startScale;

            // 1) zoom out
            float t = 0f;
            while (t < zoomDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, zoomDuration));
                float e = ease.Evaluate(k);
                animatedRect.localScale = Vector3.LerpUnclamped(sStart, sEnd, e);
                yield return null;
            }
            animatedRect.localScale = sEnd;

            // pauza
            if (pauseBetween > 0f)
                yield return new WaitForSecondsRealtime(pauseBetween);

            // 2) pohyb dolù
            t = 0f;
            while (t < moveDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, moveDuration));
                float e = ease.Evaluate(k);
                animatedRect.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, e);
                yield return null;
            }
            animatedRect.anchoredPosition = endPos;
        }
        else if (animatedTransform != null)
        {
            Vector3 startPos = animatedTransform.localPosition;
            Vector3 endPos = transformTargetLocalPos + new Vector3(0f, startYOffset * 0.01f, 0f);
            Vector3 sStart = animatedTransform.localScale;
            Vector3 sEnd = Vector3.one * startScale;

            // 1) zoom out
            float t = 0f;
            while (t < zoomDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, zoomDuration));
                float e = ease.Evaluate(k);
                animatedTransform.localScale = Vector3.LerpUnclamped(sStart, sEnd, e);
                yield return null;
            }
            animatedTransform.localScale = sEnd;

            // pauza
            if (pauseBetween > 0f)
                yield return new WaitForSecondsRealtime(pauseBetween);

            // 2) pohyb dolù
            t = 0f;
            while (t < moveDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, moveDuration));
                float e = ease.Evaluate(k);
                animatedTransform.localPosition = Vector3.LerpUnclamped(startPos, endPos, e);
                yield return null;
            }
            animatedTransform.localPosition = endPos;
        }

        // deaktivovat UI
        if (monitorUI != null) monitorUI.SetActive(false);
        monitorOpen = false;

        // obnovit movement pokud byl blokován
        if (blockedMovementComponent != null)
        {
            if (!TrySetBoolFieldOrProperty(blockedMovementComponent, "canMove", true))
                blockedMovementComponent.enabled = true;
            blockedMovementComponent = null;
        }

        UIManager.Instance?.HideText();
        animationCoroutine = null;
    }

    // používáme triggery místo kolizí pro interakèní range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isComputerAccesable == true)
        {
            currentPlayer = other.gameObject;
            UIManager.Instance?.ShowText("Press E to interact with pc");
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // pokud je monitor otevøený, zavøi ho a obnov pohyb
            if (monitorOpen)
                ToggleMonitor();

            currentPlayer = null;
            UIManager.Instance?.HideText();
            playerInRange = false;
        }
    }

    // Heuristika pro nalezení pohybového skriptu na hráèi (fallback)
    private Behaviour FindLikelyMovementComponent(GameObject player)
    {
        var comps = player.GetComponents<Behaviour>();
        foreach (var c in comps)
        {
            if (c == null) continue;
            var name = c.GetType().Name.ToLowerInvariant();
            if (name.Contains("move") || name.Contains("controller") || name.Contains("player"))
                return c;
        }
        // fallback: první behaviour (mimo tohoto skriptu)
        foreach (var c in comps)
        {
            if (c == null || c == this) continue;
            return c;
        }
        return null;
    }

    // Pokusí se nastavit bool pole nebo property; vrací true pokud uspìlo
    private bool TrySetBoolFieldOrProperty(object target, string name, bool value)
    {
        var type = target.GetType();
        var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null && field.FieldType == typeof(bool))
        {
            field.SetValue(target, value);
            return true;
        }

        var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop != null && prop.PropertyType == typeof(bool) && prop.CanWrite)
        {
            prop.SetValue(target, value, null);
            return true;
        }

        return false;
    }
}