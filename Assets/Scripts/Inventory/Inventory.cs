using System;
using UnityEngine;

/// <summary>
/// The player's inventory of items, also handles wealth (gold).
/// </summary>
public class Inventory : MonoBehaviour
{
    // Inventory related constants.
    private const int MaxItems = 24;
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

    // The physical models to be shown/hidden based on the weapon in the weapon slot.
    private GameObject longsword;
    private GameObject shortsword;
    private GameObject scimitar;
    private GameObject dagger;
    private GameObject axe;

    void Start()
    {
        longsword = GameObject.Find("WEPLongsword");
        shortsword = GameObject.Find("WEPShortsword");
        scimitar = GameObject.Find("WEPScimitar");
        dagger = GameObject.Find("WEPDagger");
        axe = GameObject.Find("WEPAxe");
        longsword.SetActive(false);
        shortsword.SetActive(false);
        scimitar.SetActive(false);
        dagger.SetActive(false);
        axe.SetActive(false);

        if (StateMigrator.allItems == null)
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
            items[2] = new DebugSword1();
            items[3] = new DebugSword2();
            items[4] = new DebugSword3();
            items[6] = new AmethystRingOfPower();
            items[7] = new AncientRing();
            items[8] = new BronzeRing();
            items[9] = new DyadRingOfSpeed();
            items[10] = new RingOfFortitude();
            items[11] = new DaggerOfSpeed();
            items[12] = new IronShortsword();
            items[13] = new ScimitarOfTheEast();
            items[14] = new SharpAxeOfPower();
            items[15] = new SteelLongswordOfStrength();
        }
        else
        {
            allItems = StateMigrator.allItems;
            items = allItems[(int)InventoryType.InventoryItems];
            equipped = allItems[(int)InventoryType.EquippedItems];
            wealth = StateMigrator.wealth;
            player.RecalculateStats();
        }
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

    public bool HasItem(Item item)
    {
        if (item == null)
        {
            return false;
        }
        for (int i = 0; i < MaxItems; i++)
        {
            if (items[i] != null && items[i].GetType().Equals(item.GetType()))
            {
                return true;
            }
        }
        for (int i = 0; i < (int)GearType.OneAboveMax; i++)
        {
            if (equipped[i] != null && equipped[i].GetType().Equals(item.GetType()))
            {
                return true;
            }
        }
        return false;
    }

    public bool AddItem(Item item)
    {
        Coin coin = item as Coin;
        if (coin != null)
        {
            wealth += coin.Amount;
            return true;
        }
        else
        {
            for (int i = 0; i < MaxItems; i++)
            {
                if (items[i] == null)
                {
                    items[i] = item;
                    return true;
                }
            }
        }
        return false;
    }

    public void StartMoveItem()
    {
        draggingItem = true;
        if (lastNonDragItemIndex != -1)
        {
            Item item = allItems[(int)lastNonDragInventoryType][lastNonDragItemIndex];
            if (item != null)
            {
                uiManager.ShowItemDrag(item);
            }
        }
    }

    public void EndMoveItem()
    {
        draggingItem = false;
        uiManager.CloseItemDrag();
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
                        ChangePhysicalWeapon();
                    }
                }
                else
                {
                    uiManager.DisplayMessage(string.Format("Only {0} items may go into this slot.", (GearType)lastDragItemIndex));
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
                        ChangePhysicalWeapon();
                    }
                }
                else
                {
                    if (lastDragInventoryType == InventoryType.EquippedItems)
                    {
                        uiManager.DisplayMessage(string.Format("Only {0} items may go into this slot.", (GearType)lastDragItemIndex));
                    }
                    else
                    {
                        uiManager.DisplayMessage(string.Format("Only {0} items may go into this slot.", (GearType)lastNonDragItemIndex));
                    }
                }
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

    public void SaveState()
    {
        StateMigrator.allItems = allItems;
        StateMigrator.wealth = wealth;
    }

    private void ChangePhysicalWeapon()
    {
        longsword.SetActive(false);
        shortsword.SetActive(false);
        scimitar.SetActive(false);
        dagger.SetActive(false);
        axe.SetActive(false);
        if (IsEquipped(new SteelLongswordOfStrength()))
        {
            longsword.SetActive(true);
        }
        if (IsEquipped(new IronShortsword()))
        {
            shortsword.SetActive(true);
        }
        if (IsEquipped(new ScimitarOfTheEast()))
        {
            scimitar.SetActive(true);
        }
        if (IsEquipped(new DaggerOfSpeed()))
        {
            dagger.SetActive(true);
        }
        if (IsEquipped(new SharpAxeOfPower()))
        {
            axe.SetActive(true);
        }
    }

    private bool IsEquipped(GearItem gearItem)
    {
        if (gearItem == null)
        {
            return false;
        }
        for (int i = 0; i < (int)GearType.OneAboveMax; i++)
        {
            if (equipped[i] != null && equipped[i].GetType().Equals(gearItem.GetType()))
            {
                return true;
            }
        }
        return false;
    }
}
