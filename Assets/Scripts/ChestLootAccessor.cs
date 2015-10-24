using UnityEngine;
using System.Collections;

public class ChestLootAccessor : MonoBehaviour
{
    public UIManager uiManager;
    public Inventory inventory;
    public PlayerController playerController;

    private ChestLoot chestLoot;

    void Start ()
    {
        chestLoot = new ChestLoot(uiManager, inventory);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Adventurer")
        {
            playerController.StopMoving();
            uiManager.ShowLootWindow(chestLoot);
        }
    }

    void OnTriggerExit()
    {
        uiManager.CloseLootWindow();
    }
}
