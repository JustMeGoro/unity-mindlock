using Unity.VisualScripting;
using UnityEngine;


public enum itemType    // list itemù
{
    Cogwheel
}

public class PickupItem : MonoBehaviour
{
    public static PickupItem Instance;

    [Header("Item options")]
    // volby v inspektoru
    public string itemName = "Item"; 
    public itemType itemType;
    public Sprite itemIcon;

    [Header("Text options")]
    public string interactionText;


    public AudioSource cogwheelPickupAudioSource;


    private static int playersInTrigger = 0;
    private bool playerInRange = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && InventoryUI.Instance.isHoldingItem == false)
        {
            Pickup();
        }
        else if (playerInRange && InventoryUI.Instance.isHoldingItem == true)
        {
            UIManager.Instance.ShowText("You are already holding something");
        }
    }

    private void Pickup()
    {
        Debug.Log(itemName + " was picked up!");
        UIManager.Instance.HideText();

        InventoryUI.Instance.AddItem(itemIcon);
        cogwheelPickupAudioSource.Play();

        gameObject.SetActive(false);
        InventoryUI.Instance.isHoldingItem = true;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playersInTrigger++;
            UIManager.Instance.ShowText(interactionText);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger--;
            if (playersInTrigger <= 0)
            {
                playerInRange = false;
                UIManager.Instance.HideText();
                playersInTrigger = 0;
            }
        }
    }
}
