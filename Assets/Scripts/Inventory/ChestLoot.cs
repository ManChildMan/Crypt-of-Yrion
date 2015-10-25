using UnityEngine;
using System.Collections;

public class ChestLoot : Loot
{
    public ChestLoot(UIManager uiManager, Inventory inventory)
        : base(uiManager, inventory)
    {
        items = new Item[MaxItems];
        int index = 0;
        if (StateMigrator.lastPortalActionTaken == PortalAction.GotoLevel1)
        {
            items[index++] = new Coin((int)(1100 * Random.value) + 1);
            if (Percent(3.0f))
            {
                items[index++] = new PowerfulPortalBinding();
            }
        }
        else if (StateMigrator.lastPortalActionTaken == PortalAction.GotoLevel2)
        {
            items[index++] = new Coin((int)(6500 * Random.value) + 1);
            if (Percent(2.0f))
            {
                items[index++] = new MysticPortalBinding();
            }
        }
        // Rings
        if (Percent(4.0))
        {
            items[index++] = new AmethystRingOfPower();
        }
        if (Percent(2.0))
        {
            items[index++] = new AncientRing();
        }
        if (Percent(40.0))
        {
            items[index++] = new BronzeRing();
        }
        if (Percent(10.0))
        {
            items[index++] = new DyadRingOfSpeed();
        }
        if (Percent(10.0))
        {
            items[index++] = new RingOfFortitude();
        }
        // Necklaces
        if (Percent(4.0))
        {
            items[index++] = new EnchantedLocket();
        }
        if (Percent(10.0))
        {
            items[index++] = new GoldenPendant();
        }
        if (Percent(18.0))
        {
            items[index++] = new ThornyNecklace();
        }
        // Weapons
        if (Percent(10.0))
        {
            items[index++] = new DaggerOfSpeed();
        }
        if (Percent(18.0))
        {
            items[index++] = new IronShortsword();
        }
        if (Percent(10.0))
        {
            items[index++] = new ScimitarOfTheEast();
        }
        if (Percent(4.0))
        {
            items[index++] = new SharpAxeOfPower();
        }
        if (Percent(2.0))
        {
            items[index++] = new SteelLongswordOfStrength();
        }
        // Waist
        if (Percent(18.0))
        {
            items[index++] = new BlueSash();
        }
        if (Percent(10.0))
        {
            items[index++] = new GemstoneBelt();
        }
        if (Percent(18.0))
        {
            items[index++] = new GreenSash();
        }
    }
    
    private bool Percent(double percentage)
    {
        percentage /= 100.0;
        return StateMigrator.random.NextDouble() < percentage;
    }
}
