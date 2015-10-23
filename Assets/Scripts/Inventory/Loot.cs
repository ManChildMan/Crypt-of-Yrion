using UnityEngine;
using System.Collections;

public class Loot
{
    public UIManager uiManager;
    public Inventory inventory;

    protected const int MaxItems = 10;
    protected Item[] items; // initialized by inheritor.

    public Loot(UIManager uiManager, Inventory inventory)
    {
        this.uiManager = uiManager;
        this.inventory = inventory;
    }

    public Item GetItem(int index)
    {
        return items[index];
    }

    public void TakeItem(int index)
    {
        if (inventory.HasItem(items[index]))
        {
            uiManager.DisplayMessage("You already have this item in your inventory.");
            return;
        }
        if (!inventory.AddItem(items[index]))
        {
            uiManager.DisplayMessage("Your inventory is full.");
            return;
        }
    }
}
