using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PipeInteract : MonoBehaviour
{
    public static List<PipeInteract> allPipes = new List<PipeInteract>();

    public AudioSource metalClank;

    public TMP_Text interactionText;       // UI text "Press E to place cogwheel"
    public string playerTag = "Player";

    public GameObject cogwheelOnPipe;
    private bool playerInRange = false;

    public bool cogwheelInPlace = false;
    public int cogwheelPlaced = 0;

    void Start()
    {
        allPipes.Add(this);

        // cogwheel zpoèátku skrytý
        cogwheelOnPipe.SetActive(false);

        // Text zpoèátku skrytý
        if (interactionText != null)
            interactionText.enabled = false;
    }

    void OnDestroy()
    {
        allPipes.Remove(this);
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
                Debug.Log("Entered range");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E pressed");
                cogwheelOnPipe.SetActive(true);
                cogwheelInPlace = true;
                PlayerInteractor.Instance.isHoldingItem = false;
                cogwheelPlaced++;
                metalClank.Play();
                
            }
        }
        else
        {
            // pokud hráè odejde, skryj text
            if (interactionText != null && interactionText.enabled)
                interactionText.enabled = false;
            Debug.Log("Exited range");
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
