using System;
using UnityEngine;

/// <summary>
/// The player's inventory of items, also handles wealth (gold).
/// </summary>
public class Inventory : MonoBehaviour
{
    // Inventory related constants.
    private const int MaxItems = 16;
    private const int WeathStart = 1000;

    // References set by unity.
    public UIManager uiManager;
    public Player player;

    // Item and wealth fields.
    private Item[] items;
    private Item[] equipped;
    private Item[][] allItems;
    private int wealth;

    // Fields used for dragging items.
    private int lastNonDragItemIndex = -1;
    private InventoryType lastNonDragInventoryType;
    private int lastDragItemIndex = -1;
    private InventoryType lastDragInventoryType;
    private bool draggingItem;

    void Start()
    {
        items = new Item[MaxItems];
        equipped = new Item[(int)GearType.OneAboveMax]; // One slot for each gear type.
        allItems = new Item[(int)InventoryType.OneAboveMax][];
        allItems[(int)InventoryType.InventoryItems] = items;
        allItems[(int)InventoryType.EquippedItems] = equipped;
        wealth = WeathStart;

        // Remove later.
        items[0] = new MagicalOrb();
        items[1] = new TheOneRing();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if (!draggingItem && lastNonDragItemIndex != -1)
            {
                Item item = allItems[(int)lastNonDragInventoryType][lastNonDragItemIndex];
                GeneralItem generalItem = item as GeneralItem;
                if (generalItem != null)
                {
                    generalItem.Use();
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the wealth (gold) of the inventory.
    /// </summary>
    public int Wealth
    {
        get
        {
            return wealth;
        }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException();
            }
            wealth = value;
        }
    }

    /// <summary>
    /// Retrives an item in the inventory.
    /// </summary>
    /// <param name="index">The inventory type to retrieve an item from.</param>
    /// <param name="index">The item index of the inventory to retrieve.</param>
    /// <returns>The item, returns null if there is no item at the specified index.</returns>
    public Item GetItem(InventoryType inventoryType, int index)
    {
        switch (inventoryType)
        {
            case InventoryType.InventoryItems:
                if (index < 0 || index >= MaxItems)
                {
                    throw new IndexOutOfRangeException();
                }
                break;
            case InventoryType.EquippedItems:
                if (index < 0 || index >= (int)GearType.OneAboveMax)
                {
                    throw new IndexOutOfRangeException();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return allItems[(int)inventoryType][index];
    }

    public void StartMoveItem()
    {
        draggingItem = true;
    }

    public void EndMoveItem()
    {
        draggingItem = false;
        if (lastNonDragItemIndex != -1 && lastDragItemIndex != -1 && allItems[(int)lastNonDragInventoryType][lastNonDragItemIndex] != null)
        {
            if (allItems[(int)lastDragInventoryType][lastDragItemIndex] == null)
            {
                Item itemToMove = allItems[(int)lastNonDragInventoryType][lastNonDragItemIndex];
                GearItem gearItemToMove = itemToMove as GearItem;
                // Ensures ring can only go into ring slot, etc...
                if (lastDragInventoryType == InventoryType.InventoryItems ||
                    (lastDragInventoryType == InventoryType.EquippedItems && gearItemToMove != null && (int)gearItemToMove.Type == lastDragItemIndex))
                {
                    allItems[(int)lastDragInventoryType][lastDragItemIndex] = itemToMove;
                    allItems[(int)lastNonDragInventoryType][lastNonDragItemIndex] = null;
                    if (lastDragInventoryType == InventoryType.EquippedItems || lastNonDragInventoryType == InventoryType.EquippedItems)
                    {
                        player.RecalculateStats();
                    }
                }
            }
            else
            {
                Item itemToMove = allItems[(int)lastNonDragInventoryType][lastNonDragItemIndex];
                Item itemToSwap = allItems[(int)lastDragInventoryType][lastDragItemIndex];
                GearItem gearItemToMove = itemToMove as GearItem;
                GearItem gearItemToSwap = itemToSwap as GearItem;
                // Ensures ring can only go into ring slot, etc...
                if ((lastDragInventoryType == InventoryType.InventoryItems ||
                    (lastDragInventoryType == InventoryType.EquippedItems && gearItemToMove != null && (int)gearItemToMove.Type == lastDragItemIndex)) &&
                    (lastNonDragInventoryType == InventoryType.InventoryItems ||
                    lastNonDragInventoryType == InventoryType.EquippedItems && gearItemToSwap != null && (int)gearItemToSwap.Type == lastNonDragItemIndex))
                {
                    allItems[(int)lastDragInventoryType][lastDragItemIndex] = itemToMove;
                    allItems[(int)lastNonDragInventoryType][lastNonDragItemIndex] = itemToSwap;
                    if (lastDragInventoryType == InventoryType.EquippedItems || lastNonDragInventoryType == InventoryType.EquippedItems)
                    {
                        player.RecalculateStats();
                    }
                };
            }
        }
        lastNonDragItemIndex = lastDragItemIndex;
        lastNonDragInventoryType = lastDragInventoryType;
        lastDragItemIndex = -1;
    }

    public void InventorySlotOverMoveItem(int index)
    {
        SlotOverMoveItem(InventoryType.InventoryItems, index);
    }

    public void EquipSlotOverMoveItem(int index)
    {
        SlotOverMoveItem(InventoryType.EquippedItems, index);
    }

    private void SlotOverMoveItem(InventoryType inventoryType, int index)
    {
        if (!draggingItem)
        {
            lastNonDragItemIndex = index;
            lastNonDragInventoryType = inventoryType;
            Item item = allItems[(int)inventoryType][index];
            if (item != null)
            {
                uiManager.ShowItemPreviewWindow(item);
            }
        }
        else
        {
            lastDragItemIndex = index;
            lastDragInventoryType = inventoryType;
        }
    }

    public void SlotLeaveMoveItem()
    {
        if (!draggingItem)
        {
            lastNonDragItemIndex = -1;
        }
        else
        {
            lastDragItemIndex = -1;
        }
        uiManager.CloseItemPreviewWindow();
    }
}
