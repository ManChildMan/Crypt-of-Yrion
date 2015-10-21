using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used for applying the correct item images to shop item slots.
/// </summary>
public class ShopItemSlot : MonoBehaviour
{
    // References/fields set by unity.
    public UIManager uiManager;
    public ItemImages itemImages;
    public GameObject itemCostObject;
    public GameObject purchaseButtonObject;
    public int index;

    private Shop shop;
    
    public void SetShop(Shop shop)
    {
        this.shop = shop;
        Item item = shop.GetItem(index);
        if (item == null)
        {
            itemCostObject.SetActive(false);
            purchaseButtonObject.SetActive(false);
            transform.GetChild(0).GetComponent<Image>().sprite = itemImages.GetItemSprite("Null");
        }
        else
        {
            itemCostObject.SetActive(true);
            purchaseButtonObject.SetActive(true);
            transform.GetChild(0).GetComponent<Image>().sprite = itemImages.GetItemSprite(item.Name);
            itemCostObject.GetComponent<Text>().text = shop.GetItemCost(index).ToString();
        }
    }

    public void MouseOver()
    {
        Item item = shop.GetItem(index);
        if (item != null)
        {
            uiManager.ShowItemPreviewWindow(shop.GetItem(index));
        }
    }

    public void MouseLeave()
    {
        uiManager.CloseItemPreviewWindow();
    }

    public void BuyItem()
    {
        shop.BuyItem(index);
    }
}
