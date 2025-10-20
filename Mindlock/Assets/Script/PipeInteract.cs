using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PipeInteract : MonoBehaviour
{
    public static List<PipeInteract> allPipes = new List<PipeInteract>();

    public AudioSource metalClank;

    public string interactionText;       // UI text "Press E to place cogwheel"
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
    }

    void OnDestroy()
    {
        allPipes.Remove(this);
    }

    void Update()
    {
        if (playerInRange && InventoryUI.Instance.isHoldingItem == true && Input.GetKeyDown(KeyCode.E) && cogwheelInPlace == false)
        {
            PlaceOnPipe();
        }
    }

    void PlaceOnPipe()
    {
        metalClank.Play();
        InventoryUI.Instance.slot.enabled = false;
        InventoryUI.Instance.isHoldingItem = false;
        UIManager.Instance.HideText();
        cogwheelOnPipe.SetActive(true);
        cogwheelInPlace = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && cogwheelInPlace == false)
        {
            playerInRange = true;
            UIManager.Instance.ShowText(interactionText);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            UIManager.Instance.HideText();
        }
    }
}
