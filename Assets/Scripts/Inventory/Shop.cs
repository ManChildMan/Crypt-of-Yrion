using UnityEngine;
using System.Collections;

public class Shop
{
    public UIManager uiManager;
    public Inventory inventory;

    protected const int MaxItems = 10;
    protected Item[] items; // initialized by inheritor.
    protected int[] costs; // initialized by inheritor.

    public Shop(UIManager uiManager, Inventory inventory)
    {
        this.uiManager = uiManager;
        this.inventory = inventory;
    }

    public Item GetItem(int index)
    {
        return items[index];
    }

    public int GetItemCost(int index)
    {
        return costs[index];
    }

    public void BuyItem(int index)
    {
        if (inventory.Wealth < costs[index])
        {
            uiManager.DisplayMessage("You do not have enough gold to purchase this item.");
            return;
        }
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
        inventory.Wealth -= costs[index];
    }
}
