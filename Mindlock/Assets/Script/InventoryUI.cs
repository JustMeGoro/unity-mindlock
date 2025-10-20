using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public bool isHoldingItem = false;

    public static InventoryUI Instance; // vytvoøíme promìnnou s odkazem na tento script
    public Image slot;

    void Awake()
    {
        Instance = this; // pøi startu uložíme odkaz na tento skript do instance
    }

    private void Start()
    {
        slot.enabled = false; // obrázek ve slotu zneviditelní
    }

    public void AddItem(Sprite newItem)
    {
        slot.sprite = newItem;   // nastav ikonku
        slot.enabled = true;     // zviditelni 
        isHoldingItem = true;
    }
}
