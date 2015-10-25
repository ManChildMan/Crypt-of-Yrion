
using System;

public class StateMigrator
{
    public static PortalAction lastPortalActionTaken;
    public static Item[][] allItems;
    public static int wealth;
    public static Random random;
    public static bool anyWindowOpen;
    public static int deathCounter;
    public static float gameTimer;

    static StateMigrator()
    {
        random = new Random();
    }
}
