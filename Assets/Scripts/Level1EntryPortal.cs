using UnityEngine;
using System.Collections;

public class Level1EntryPortal : MonoBehaviour
{
    public UIManager uiManager;
    public Inventory inventory;

	void OnTriggerEnter (Collider other)
    {
        if (other.name == "Adventurer")
        {
            if (inventory.HasItem(new PortalBinding()))
            {
                uiManager.ShowPortalDialog("Take the portal to the level 1 dungeon?", PortalAction.GotoLevel1);
            }
            else
            {
                uiManager.DisplayMessage("A Portal Binding is required to use this Portal. This can be obtained from the merchant.");
            }
        }
    }

    void OnTriggerExit()
    {
        uiManager.ClosePortalDialog();
    }
}
