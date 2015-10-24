using UnityEngine;
using System.Collections;

public class ChestLoot : Loot
{
    public ChestLoot(UIManager uiManager, Inventory inventory)
        : base(uiManager, inventory)
    {
        items = new Item[MaxItems];
        items[0] = new PortalBinding();
        items[1] = new Coin(10000);
    }
}
