using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the wealth/gold value in inventory window when necessary.
/// </summary>
public class WealthText : MonoBehaviour
{
    // References set by unity.
    public Inventory inventory;

    private int prevWealth;
    private Text wealthText;

    void Start()
    {
        wealthText = GetComponent<Text>();
    }
	
	void Update ()
    {
        int wealth = inventory.Wealth;
        if (prevWealth != wealth)
        {
            wealthText.text = string.Format("{0:n0}", wealth);
        }
	}
}
