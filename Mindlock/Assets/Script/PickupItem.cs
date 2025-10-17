using UnityEngine;

public enum itemType
{
    Cogwheel
}

public class PickupItem : MonoBehaviour, IInteractable
{
    public string itemName = "Item";
    public itemType itemType;

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

        Debug.Log("Picked up " + itemName);

        PlayerInteractor player = Object.FindFirstObjectByType<PlayerInteractor>();

        if (itemType == itemType.Cogwheel)
        {
            player.AddCogwheel();
        }

        gameObject.SetActive(false);
    }
}
