using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class PipeInteract : MonoBehaviour
{
    public TMP_Text interactionText;       // UI text "Press E to place cogwheel"
    public string playerTag = "Player";

    public GameObject cogwheelOnPipe;
    private bool playerInRange = false;

    private bool cogwheelInPlace = false;

    void Start()
    {
        // cogwheel zpoèátku skrytý
        cogwheelOnPipe.SetActive(false);

        // Text zpoèátku skrytý
        if (interactionText != null)
            interactionText.enabled = false;
    }

    void Update()
    {
        // pokud je hráè v dosahu, zobraz text
        if (playerInRange && PlayerInteractor.Instance != null && PlayerInteractor.Instance.isHoldingItem == true && cogwheelInPlace == false)
        {
            if (interactionText != null && !interactionText.enabled)
            {
                interactionText.text = "Press E to place cogwheel";
                interactionText.enabled = true;
                Debug.Log("Cogwheel was placed on a pipe");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E pressed");
                cogwheelOnPipe.SetActive(true);
                cogwheelInPlace =true;
            }
        }
        else
        {
            // pokud hráè odejde, skryj text
            if (interactionText != null && interactionText.enabled)
                interactionText.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }
}
