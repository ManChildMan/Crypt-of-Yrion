using UnityEngine;

public class Player : Entity
{
    // References set by unity.
    public Inventory inventory;

    public void RecalculateStats()
    {
        totalHealth = baseAttack;
        totalSpeed = baseSpeed;
        totalAttack = baseAttack;
        for (int i = 0; i < (int)GearType.OneAboveMax; i++)
        {
            GearItem item = (GearItem)inventory.GetItem(InventoryType.EquippedItems, i);
            if (item != null)
            {
                totalHealth += item.Health;
                totalSpeed += item.Speed;
                totalAttack += item.Attack;
            }
        }
    }
}
