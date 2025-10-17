using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    public Image slot; // jen jeden slot

    private void Start()
    {
        slot.enabled = false;
    }

    void Awake()
    {
        instance = this;
    }

    public void AddItem(Sprite newItem)
    {
        if (slot != null)
        {
            slot.sprite = newItem;   // nastav ikonku
            slot.enabled = true;     // zviditelni
        }
    }
}
