
public class Item
{
    public string Id = "";
    public string Class = "";
    public string Name = "";
    public string Description = "";
    public float Weight = 0f;
    public float Value = 0f; 
}

public class Weapon : Item
{
    public int OffenseRating = 0;
    public int DefenseRating = 0;
}

public class Potion : Item
{
}

public class Scroll : Item
{
}

public class Armour : Item
{
    public int ArmourRating = 0;
    public int AgilityModifier = 0;
}
