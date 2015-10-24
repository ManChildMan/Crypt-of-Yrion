using System;

/// <summary>
/// Represents an abstract general/usable item.
/// </summary>
public abstract class GeneralItem : Item
{
    public GeneralItem(string name, string description, Rarity rarity)
        : base(name, description, rarity)
    {
    }

    // This method should be overridden if item is usable.
    public virtual void Use()
    {
    }
}
