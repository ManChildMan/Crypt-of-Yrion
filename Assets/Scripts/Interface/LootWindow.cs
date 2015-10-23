using UnityEngine;
using System.Collections;

public class LootWindow : MonoBehaviour
{
    public LootItemSlot[] slots;

    public void ShowLoot(Loot loot)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetLoot(loot);
        }
    }
}
