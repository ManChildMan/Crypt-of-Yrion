using UnityEngine;
using System.Collections;

public class ChestLootAccessor : MonoBehaviour
{
    public UIManager uiManager;
    public Inventory inventory;

    private ChestLoot chestLoot;

    void Start ()
    {
        chestLoot = new ChestLoot(uiManager, inventory);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Adventurer")
        {
            uiManager.ShowLootWindow(chestLoot);
        }
    }

    void OnTriggerExit()
    {
        uiManager.CloseLootWindow();
    }
}
