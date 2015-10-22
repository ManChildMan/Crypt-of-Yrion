using UnityEngine;
using System.Collections;

public class Level1ExitPortal : MonoBehaviour
{

    public UIManager uiManager;
    public Inventory inventory;

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Adventurer")
        {
            uiManager.ShowPortalDialog("Take the portal back to the safe zone area?", PortalAction.ExitLevel1);
        }
    }

    void OnTriggerExit()
    {
        uiManager.ClosePortalDialog();
    }
}
