using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    public Image itemImage;

    void Start()
    {
        itemImage.enabled = false;
    }

    void Awake()
    {
        instance = this;
        itemImage.enabled = false;
    }

    public void AddItem(Sprite newItem)
    {
        itemImage.sprite = newItem;
        itemImage.enabled = true;
    }

}
