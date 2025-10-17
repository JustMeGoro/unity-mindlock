using UnityEngine;

public enum itemType
{
    Cogwheel
}

public class PickupItem : MonoBehaviour, IInteractable
{
    public string itemName = "Item";
    public itemType itemType;
    public Sprite itemIcon;

    public string GetPromptMessage()
    {
        if (PlayerInteractor.Instance.isHoldingItem == false)
        {
            return "press E to pick up " + itemName;
        }
        else
        {
            return "you are already holding something";
        }
    }

    public void Interact()
    {

        if (PlayerInteractor.Instance.isHoldingItem == true)
        {
            return;
        }

        Debug.Log("Picked up " + itemName);

        InventoryUI.instance.AddItem(itemIcon);

        PlayerInteractor.Instance.isHoldingItem = true;

        gameObject.SetActive(false);
    }
}
