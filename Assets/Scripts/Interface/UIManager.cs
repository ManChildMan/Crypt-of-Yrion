using UnityEngine;

// Manages UI on the canvas game object (includes input on the canvas, use UserInterface for world input).
public class UIManager : MonoBehaviour {
    
    private GameObject inventoryWindow;
    
	void Start () {
        inventoryWindow = transform.FindChild("InventoryWindow").gameObject;
        inventoryWindow.SetActive(false);
    }

    public void ShowInventoryWindow()
    {
        inventoryWindow.SetActive(true);
    }

    public void CloseInventoryWindow()
    {
        inventoryWindow.SetActive(false);
    }
}
