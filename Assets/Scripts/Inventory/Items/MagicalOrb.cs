using UnityEngine;

public class MagicalOrb : GeneralItem
{
    /// <summary>
    /// Item used for testing, increases wealth by 10000.
    /// </summary>
    public MagicalOrb()
        : base("Magical Orb", "Use: Right-click to instantly create 10,000 gold.", Rarity.Epic, true)
    {
    }

    public override void Use()
    {
        Inventory inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        inventory.Wealth += 10000;
    }
}
