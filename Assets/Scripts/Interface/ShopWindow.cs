using UnityEngine;

public class ShopWindow : MonoBehaviour
{
    public ShopItemSlot[] slots;

    public void ShowShop(Shop shop)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetShop(shop);
        }
    }
}
