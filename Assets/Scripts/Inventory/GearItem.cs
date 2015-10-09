using System;

/// <summary>
/// Represents an abstract gear/equippable item.
/// </summary>
public abstract class GearItem : Item
{
    private GearType type;
    private int health;
    private int speed;
    private int attack;
    public GearItem(string name, string description, Rarity rarity, GearType type, int health, int speed, int attack)
        : base(name, description, rarity)
    {
        if (type < 0 || type >= GearType.OneAboveMax)
        {
            throw new ArgumentOutOfRangeException();
        }
        this.type = type;
        this.health = health;
        this.speed = speed;
        this.attack = attack;
    }

    /// <summary>
    /// Gets the gear type of the item.
    /// </summary>
    public GearType Type
    {
        get
        {
            return type;
        }
    }

    /// <summary>
    /// Gets the health stat value increase caused when equipping this item.
    /// </summary>
    public int Health
    {
        get
        {
            return health;
        }
    }

    /// <summary>
    /// Gets the speed stat value increase caused when equipping this item.
    /// </summary>
    public int Speed
    {
        get
        {
            return speed;
        }
    }

    /// <summary>
    /// Gets the attack stat value increase caused when equipping this item.
    /// </summary>
    public int Attack
    {
        get
        {
            return attack;
        }
    }
}