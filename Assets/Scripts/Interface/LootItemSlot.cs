using UnityEngine;
using UnityEngine.UI;

public class LootItemSlot : MonoBehaviour
{
    public UIManager uiManager;
    public ItemImages itemImages;
    public GameObject takeButtonObject;
    public int index;

    private Loot loot;

    public void SetLoot(Loot loot)
    {
        this.loot = loot;
        Item item = loot.GetItem(index);
        if (item == null)
        {
            takeButtonObject.SetActive(false);

        }
        else
        {
            takeButtonObject.SetActive(true);
            transform.GetChild(0).GetComponent<Image>().sprite = itemImages.GetItemSprite(item.Name);
        }
    }

    public void MouseOver()
    {
        Item item = loot.GetItem(index);
        if (item != null)
        {
            uiManager.ShowItemPreviewWindow(loot.GetItem(index));
        }
    }

    public void MouseLeave()
    {
        uiManager.CloseItemPreviewWindow();
    }

    public void TakeItem()
    {
        loot.TakeItem(index);
    }
}
