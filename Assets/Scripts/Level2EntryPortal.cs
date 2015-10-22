using UnityEngine;
using System.Collections;

public class Level2EntryPortal : MonoBehaviour
{
    public UIManager uiManager;
    public Inventory inventory;

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Adventurer")
        {
            if (inventory.HasItem(new PowerfulPortalBinding()))
            {
                uiManager.ShowPortalDialog("Take the portal to the level 2 dungeon?", PortalAction.GotoLevel2);
            }
            else
            {
                uiManager.DisplayMessage("A Powerful Portal Binding is required to use this Portal.");
            }
        }
    }

    void OnTriggerExit()
    {
        uiManager.ClosePortalDialog();
    }
}
