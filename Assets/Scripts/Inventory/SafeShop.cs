using UnityEngine;
using System.Collections;

public class SafeShop : Shop
{
    public SafeShop(UIManager uiManager, Inventory inventory)
        : base(uiManager, inventory)
    {
        items = new Item[MaxItems];
        items[0] = new PortalBinding();
        items[1] = new PowerfulPortalBinding();
        items[2] = new MysticPortalBinding();
        costs = new int[MaxItems];
        costs[0] = 100;
        costs[1] = 5000;
        costs[2] = 20000;
    }
}
