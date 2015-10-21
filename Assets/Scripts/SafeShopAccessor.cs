using UnityEngine;
using System.Collections;

public class SafeShopAccessor : MonoBehaviour
{
    public UIManager uiManager;
    public Inventory inventory;
    public PlayerController playerController;
    private SafeShop safeShop;

    void Start()
    {
        safeShop = new SafeShop(uiManager, inventory);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Adventurer")
        {
            playerController.StopMoving();
            uiManager.ShowShopWindow(safeShop);
        }
    }

    void OnTriggerExit()
    {
        uiManager.CloseShopWindow();
    }
}
