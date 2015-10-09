using System;

/// <summary>
/// Represents an abstract item.
/// </summary>
public abstract class Item
{
    private string name;
    private string description;
    private Rarity rarity;

    public Item(string name, string description, Rarity rarity)
    {
        if (name == null)
        {
            throw new NullReferenceException();
        }
        if (description == null)
        {
            throw new NullReferenceException();
        }
        if (rarity < 0 || rarity >= Rarity.OneAboveMax)
        {
            throw new ArgumentOutOfRangeException();
        }
        this.name = name;
        this.description = description;
        this.rarity = rarity;
    }

    /// <summary>
    /// Gets the name of the item.
    /// </summary>
    public string Name
    {
        get
        {
            return name;
        }
    }

    /// <summary>
    /// Gets the description of the item.
    /// </summary>
    public string Description
    {
        get
        {
            return description;
        }
    }

    /// <summary>
    /// Gets the rarity of the item.
    /// </summary>
    public Rarity Rarity
    {
        get
        {
            return rarity;
        }
    }
}