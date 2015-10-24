
using System;

public class StateMigrator
{
    public static PortalAction lastPortalActionTaken;
    public static Item[][] allItems;
    public static int wealth;
    public static Random random;

    static StateMigrator()
    {
        random = new Random();
    }
}
