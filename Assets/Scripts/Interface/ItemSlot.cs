using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used for applying the correct item images to item slots.
/// </summary>
public class ItemSlot : MonoBehaviour
{
    // References/fields set by unity.
    public Inventory inventory;
    public ItemImages itemImages;

    public InventoryType inventoryType;
    public int index;
    private Item prevItem;
    private Image image;

    void Start()
    {
        image = transform.GetChild(0).GetComponent<Image>();
    }

    void Update ()
    {
        Item item = inventory.GetItem(inventoryType, index);
        if (prevItem != item)
        {
            prevItem = item;
            if (item == null)
            {
                image.sprite = itemImages.GetItemSprite("Null");
            }
            else
            {
                image.sprite = itemImages.GetItemSprite(item.Name);
            }
        }
	}
}
