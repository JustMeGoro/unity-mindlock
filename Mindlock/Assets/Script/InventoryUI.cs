using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance; // vytvoøíme promìnnou s odkazem na tento script
    public Image slot; 

    private void Start()
    {
        slot.enabled = false; // obrázek ve slotu zneviditelní
    }

    void Awake()
    {
        instance = this; // pøi startu uložíme odkaz na tento skript do instance
    }

    public void AddItem(Sprite newItem)
    {
        if (slot != null) // když je slot prázdný tak...
        {
            slot.sprite = newItem;   // nastav ikonku
            slot.enabled = true;     // zviditelni 
        }
    }
}
