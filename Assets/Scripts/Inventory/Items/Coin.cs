class Coin : GeneralItem
{
    private int amount;

    public Coin(int amount)
        : base("Coin", string.Format("Represents {0:n0} gold coins.", amount), Rarity.Uncommon)
    {
        this.amount = amount;
    }

    public int Amount
    {
        get
        {
            return amount;
        }
    }
}