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

    // Item and wealth fields.
    private Item[] items;
    private int wealth;

    void Start()
    {
        items = new Item[MaxItems];
        items[0] = new MagicalOrb();
        wealth = WeathStart;
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
    /// <param name="index">The item index of the inventory to retrieve.</param>
    /// <returns>The item, returns null if there is no item at the specified index.</returns>
    public Item GetItem(int index)
    {
        if (index < 0 || index >= MaxItems)
        {
            throw new IndexOutOfRangeException();
        }
        return items[index];
    }
}
