using UnityEngine;


public enum itemType    // list itemù
{
    Cogwheel
}

public class PickupItem : MonoBehaviour, IInteractable
{
    // volby v inspektoru
    public string itemName = "Item"; 
    public itemType itemType;
    public Sprite itemIcon;

    public string GetPromptMessage()
    {
        if (PlayerInteractor.Instance.isHoldingItem == false) //pokud postava nic nedrží tak...
        {
            return "press E to pick up " + itemName; // napíše jaký item mùže postava vzít
        }
        else
        {
            return "you are already holding something"; // napíše že už nìco postava drží
        }
    }

    public void Interact()
    {

        if (PlayerInteractor.Instance.isHoldingItem == true) // pokud nìco postava drží tak se okamžitì vráti
        {
            return;
        }

        Debug.Log("Picked up " + itemName); 

        InventoryUI.instance.AddItem(itemIcon); // pøidá do slotu obrázek itemu

        PlayerInteractor.Instance.isHoldingItem = true; // zmìní bool že postava nìco drží

        gameObject.SetActive(false); // item zmizí ze zemì
    }
}
