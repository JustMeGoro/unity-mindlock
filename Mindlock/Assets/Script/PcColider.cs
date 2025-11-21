using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class PcColider : MonoBehaviour
{
    public GameObject interactionText;
    public string prompt = "Press E to open keypad";
    public string playerTag = "Player";

    [FormerlySerializedAs("consoleRoot")]
    public GameObject keypadPanel;

    bool playerInside = false;
    bool isPanelOpen = false;

    void Start()
    {
        if (interactionText != null)
            interactionText.SetActive(false);
    }

    // 3D trigger
    void OnTriggerEnter(Collider other)
    {
        HandleEnter(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        HandleExit(other.gameObject);
    }

    // 2D trigger (pokud používáš 2D collidery)
    void OnTriggerEnter2D(Collider2D other)
    {
        HandleEnter(other.gameObject);
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
            SetInteractionText(prompt);
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

        if (isPanelOpen == false && Input.GetKeyDown(KeyCode.E))
        {
            OpenPanel();
        }
        else if (isPanelOpen == true && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    void OpenPanel()
    {
        Debug.Log("PcColider: E stisknuto");
        GameObject target = keypadPanel != null ? keypadPanel : transform.parent != null ? transform.parent.gameObject : null;
        if (target != null)
            target.SetActive(true);
        if (interactionText != null)
            interactionText.SetActive(false);

        isPanelOpen = true;
        PauseMenu.Instance.canPause = false;
        PlayerMovement.Instance.canMove = false;
    }

    void ClosePanel()
    {
        GameObject target = keypadPanel != null ? keypadPanel : transform.parent != null ? transform.parent.gameObject : null;
        if (target != null)
            target.SetActive(false);
        if (interactionText != null)
            interactionText.SetActive(true);

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
}
