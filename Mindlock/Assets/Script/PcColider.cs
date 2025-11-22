using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class PcColider : MonoBehaviour
{
    public GameObject interactionText;
    public string prompt = "Press E to open keypad";
    public string prompt2 = "You already finished this task"; // nový prompt pro dokonèený úkol
    public string playerTag = "Player";

    [FormerlySerializedAs("consoleRoot")]
    public CanvasGroup keypadPanel;

    bool playerInside = false;
    bool isPanelOpen = false;
    public bool taskFinished = false;

    void Start()
    {
        keypadPanel.alpha = 0;
        if (interactionText != null)
            interactionText.SetActive(false);

        var keypadController = keypadPanel.GetComponentInChildren<KeypadController>();
        if (keypadController != null)
            keypadController.OnCorrectCode += HandleCorrectCode;

        Application.logMessageReceived += OnLogMessageReceived;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    void OnLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (condition.Contains("Correct"))
        {
            StartCoroutine(ClosePanelAfterDelay(2.0f));
        }
    }

    // 2D trigger pouze!
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!taskFinished)
        {
            HandleEnter(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        HandleExit(other.gameObject);
    }

    void HandleEnter(GameObject other)
    {
        Debug.Log($"PcColider: Enter by {other.name} (tag={other.tag})");
        if (!other.CompareTag(playerTag)) return;

        playerInside = true;
        if (interactionText != null)
        {
            // Zde se vždy nastaví správný prompt podle stavu úkolu
            SetInteractionText(taskFinished ? prompt2 : prompt);
            interactionText.SetActive(true);
            Debug.Log("PcColider: interactionText activated");
        }
        else
        {
            Debug.LogWarning("PcColider: interactionText není pøiøazen v inspektoru!");
        }
    }

    void HandleExit(GameObject other)
    {
        Debug.Log($"PcColider: Exit by {other.name} (tag={other.tag})");
        if (!other.CompareTag(playerTag)) return;

        playerInside = false;
        if (interactionText != null)
            interactionText.SetActive(false);
    }

    void Update()
    {
        if (!playerInside) return;

        // Panel lze otevøít pouze pokud není dokonèený úkol
        if (!isPanelOpen && !taskFinished && Input.GetKeyDown(KeyCode.E))
        {
            OpenPanel();
        }
        else if (isPanelOpen && Input.GetKeyDown(KeyCode.E))
        {
            ClosePanel();
        }
    }

    void OpenPanel()
    {
        if (keypadPanel != null)
            keypadPanel.gameObject.SetActive(true);

        Debug.Log("PcColider: E stisknuto");
        if (keypadPanel != null)
            keypadPanel.alpha = 1;
        else
            Debug.LogError("PcColider: keypadPanel není pøiøazen v inspektoru!");

        if (interactionText != null)
            interactionText.SetActive(false);

        isPanelOpen = true;
        PauseMenu.Instance.canPause = false;
        PlayerMovement.Instance.canMove = false;
    }

    void ClosePanel()
    {
        if (keypadPanel != null)
            keypadPanel.gameObject.SetActive(false);



        isPanelOpen = false;
        PauseMenu.Instance.canPause = true;
        PlayerMovement.Instance.canMove = true;
    }

    void SetInteractionText(string text)
    {
        if (interactionText == null) return;

        var txt = interactionText.GetComponent<Text>() ?? interactionText.GetComponentInChildren<Text>();
        if (txt != null)
        {
            txt.text = text;
            return;
        }

        var tmp = interactionText.GetComponent<TextMeshProUGUI>() ?? interactionText.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = text;
            return;
        }

        Debug.LogWarning("PcColider: interactionText GameObject neobsahuje Text ani TextMeshProUGUI komponentu.");
    }

    void HandleCorrectCode()
    {
        taskFinished = true;
        StartCoroutine(ClosePanelAfterDelay(2.0f));
    }

    IEnumerator ClosePanelAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ClosePanel();
        // Po zavøení panelu nastavíme prompt2, pokud je hráè uvnitø
        if (playerInside && interactionText != null)
        {
            SetInteractionText(prompt2);
            interactionText.SetActive(true);
        }
    }
}
