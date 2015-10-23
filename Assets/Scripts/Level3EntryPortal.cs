using UnityEngine;
using System.Collections;

public class Level3EntryPortal : MonoBehaviour {

    public UIManager uiManager;
    public Inventory inventory;

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Adventurer")
        {
            if (inventory.HasItem(new MysticPortalBinding()))
            {
                uiManager.ShowPortalDialog("Take the portal to the final dungeon?", PortalAction.GotoLevel3);
            }
            else
            {
                uiManager.DisplayMessage("A Mystic Portal Binding is required to use this Portal.");
            }
        }
    }

    void OnTriggerExit()
    {
        uiManager.ClosePortalDialog();
    }
}
