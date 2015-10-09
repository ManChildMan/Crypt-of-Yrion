using System;

/// <summary>
/// Represents an abstract general/usable item.
/// </summary>
public abstract class GeneralItem : Item
{
    private bool usable;
    public GeneralItem(string name, string description, Rarity rarity, bool usable)
        : base(name, description, rarity)
    {
        this.usable = usable;
    }

    // If usable is set as true this method should be overridden.
    public virtual void Use()
    {
    }
}
