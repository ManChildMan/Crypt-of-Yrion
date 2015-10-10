using UnityEngine;
using UnityEngine.UI;

// Manages UI on the canvas game object (includes input on the canvas, use UserInterface for world input).
public class UIManager : MonoBehaviour
{
    private GameObject inventoryWindow;
    private GameObject statsWindow;
    private GameObject equipmentWindow;
    private GameObject itemPreviewWindow;
    private bool itemPreviewWindowOpen;
    
	void Start () {
        inventoryWindow = transform.FindChild("Windows/InventoryWindow").gameObject;
        inventoryWindow.SetActive(false);
        statsWindow = transform.FindChild("Windows/StatsWindow").gameObject;
        statsWindow.SetActive(false);
        equipmentWindow = transform.FindChild("Windows/EquipmentWindow").gameObject;
        equipmentWindow.SetActive(false);
        itemPreviewWindow = transform.FindChild("ItemPreviewWindow").gameObject;
        itemPreviewWindow.SetActive(false);
    }

    void Update()
    {
        if (itemPreviewWindowOpen)
        {
            itemPreviewWindow.transform.position = new Vector3(Input.mousePosition.x + 200.0f, Input.mousePosition.y, 0.0f);
        }
    }

    public void ShowInventoryWindow()
    {
        inventoryWindow.SetActive(!inventoryWindow.activeSelf);
    }

    public void CloseInventoryWindow()
    {
        inventoryWindow.SetActive(false);
    }

    public void ShowStatsWindow()
    {
        statsWindow.SetActive(!statsWindow.activeSelf);
    }

    public void CloseStatsWindow()
    {
        statsWindow.SetActive(false);
    }

    public void ShowEquipmentWindow()
    {
        equipmentWindow.SetActive(!equipmentWindow.activeSelf);
    }

    public void CloseEquipmentWindow()
    {
        equipmentWindow.SetActive(false);
    }

    public void ShowItemPreviewWindow(Item item)
    {
        itemPreviewWindowOpen = true;
        itemPreviewWindow.SetActive(true);
        GameObject titleTextObject = itemPreviewWindow.transform.FindChild("Title/TitleText").gameObject;
        GameObject equippableTextObject = itemPreviewWindow.transform.FindChild("Title/EquippableText").gameObject;
        GameObject gearTypeTextObject = itemPreviewWindow.transform.FindChild("GearTypeText").gameObject;
        GameObject healthTextObject = itemPreviewWindow.transform.FindChild("HealthText").gameObject;
        GameObject speedTextObject = itemPreviewWindow.transform.FindChild("SpeedText").gameObject;
        GameObject attackTextObject = itemPreviewWindow.transform.FindChild("AttackText").gameObject;
        GameObject descriptionTextObject = itemPreviewWindow.transform.FindChild("DescriptionText").gameObject;

        Text titleText = titleTextObject.GetComponent<Text>();
        titleText.text = item.Name;
        switch (item.Rarity)
        {
            case Rarity.Junk:
                titleText.color = Color.gray;
                break;
            case Rarity.Common:
                titleText.color = Color.white;
                break;
            case Rarity.Uncommon:
                titleText.color = Color.green;
                break;
            case Rarity.Rare:
                titleText.color = Color.blue;
                break;
            case Rarity.Epic:
                titleText.color = Color.magenta;
                break;
        }

        GearItem gearItem = item as GearItem;
        if (gearItem != null)
        {
            equippableTextObject.SetActive(true);
            gearTypeTextObject.SetActive(true);
            gearTypeTextObject.GetComponent<Text>().text = gearItem.Type.ToString();
            if (gearItem.Health != 0)
            {
                healthTextObject.SetActive(true);
                healthTextObject.GetComponent<Text>().text = string.Format("+{0:n0} Health", gearItem.Health);
            }
            if (gearItem.Speed != 0)
            {
                speedTextObject.gameObject.SetActive(true);
                speedTextObject.GetComponent<Text>().text = string.Format("+{0:n0} Speed", gearItem.Speed);
            }
            if (gearItem.Attack != 0)
            {
                attackTextObject.gameObject.SetActive(true);
                attackTextObject.GetComponent<Text>().text = string.Format("+{0:n0} Attack", gearItem.Speed);
            }
        }
        else
        {
            equippableTextObject.SetActive(false);
            gearTypeTextObject.SetActive(false);
            healthTextObject.SetActive(false);
            speedTextObject.gameObject.SetActive(false);
            attackTextObject.gameObject.SetActive(false);
        }
        if (item.Description != "")
        {
            descriptionTextObject.SetActive(true);
            descriptionTextObject.GetComponent<Text>().text = item.Description;
        }
        else
        {
            descriptionTextObject.SetActive(false);
        }
    }

    public void CloseItemPreviewWindow()
    {
        itemPreviewWindowOpen = false;
        itemPreviewWindow.SetActive(false);
    }
}
