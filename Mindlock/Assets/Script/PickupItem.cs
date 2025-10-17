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
        return "press E to pick up " + itemName;
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
