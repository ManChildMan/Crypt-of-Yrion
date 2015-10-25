using UnityEngine;

public class Player : Entity
{
    // References set by unity.
    public Inventory inventory;

    public override void Start()
    {
        base.Start();
    }

    public void RecalculateStats()
    {
        totalHealth = baseHealth;
        totalSpeed = baseSpeed;
        totalAttack = baseAttack;
        for (int i = 0; i < (int)GearType.OneAboveMax; i++)
        {
            Item item = inventory.GetItem(InventoryType.EquippedItems, i);
            if (item != null)
            {
                GearItem gearItem = item as GearItem;
                if (item != null)
                {
                    totalHealth += gearItem.Health;
                    totalSpeed += gearItem.Speed;
                    totalAttack += gearItem.Attack;
                }
            }
        }
    }


}
