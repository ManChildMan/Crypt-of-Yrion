using UnityEngine;
using UnityEngine.UI;

// Manages UI on the canvas game object (includes input on the canvas, use UserInterface for world input).
public class UIManager : MonoBehaviour
{
    public ItemImages itemImages;

    public GameObject transitionImageObject;
    private Image transitionImage;
    private float transition = 1.0f;

    public GameObject inventoryWindow;
    public GameObject statsWindow;
    public GameObject equipmentWindow;
    public GameObject shopWindow;
    private ShopWindow shopWindowC;
    public GameObject itemPreviewWindow;
    private bool itemPreviewWindowOpen;

    public GameObject itemDragContainerObject;
    private Image itemDragImage;
    private bool itemDragOpen;

    public GameObject messageImageObject;
    private Image messageImage;
    private Text messageText;
    private bool messageShowing;
    private float messageDuration;
    private float messageShowRatio;
    
	void Start () {
        transitionImage = transitionImageObject.GetComponent<Image>();
        transitionImage.color = new Color(0.0f, 0.0f, 0.0f, 1f);
        transitionImageObject.SetActive(true);

        shopWindowC = shopWindow.GetComponent<ShopWindow>();

        itemDragImage = itemDragContainerObject.transform.FindChild("ItemDragImage").gameObject.GetComponent<Image>();

        messageImage = messageImageObject.GetComponent<Image>();
        messageImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        messageText = messageImageObject.transform.FindChild("MessageText").gameObject.GetComponent<Text>();
        messageText.color = new Color(1.0f, 0.7f, 0.0f, 0.0f);
    }

    void Update()
    {
        // Update transiton.
        if (transition != 0.0f)
        {
            transition -= Time.deltaTime;
            if (transition <= 0.0f)
            {
                transition = 0.0f;
                transitionImageObject.SetActive(false);
            }
            else
            {
                transitionImage.color = new Color(0.0f, 0.0f, 0.0f, transition);
            }
        }
        
        // Move the item preview window if its open.
        if (itemPreviewWindowOpen)
        {
            itemPreviewWindow.transform.position = new Vector3(Input.mousePosition.x + 200.0f, Input.mousePosition.y, 0.0f);
        }
        
        // Move the item drag image if its open.
        if (itemDragOpen)
        {
            itemDragContainerObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        }

        // Show/hide/transition a message.
        if (messageShowing)
        {
            messageDuration -= Time.deltaTime;
            if (messageDuration <= 0.0f)
            {
                messageShowing = false;
            }
            else if (messageShowRatio != 1.0f)
            {
                messageShowRatio += Time.deltaTime;
                if (messageShowRatio > 1.0f)
                {
                    messageShowRatio = 1.0f;
                }
                messageImage.color = new Color(1.0f, 1.0f, 1.0f, messageShowRatio);
                messageText.color = new Color(1.0f, 0.7f, 0.0f, messageShowRatio);
            }
        }
        else if (messageShowRatio != 0.0f)
        {
            messageShowRatio -= Time.deltaTime;
            if (messageShowRatio <= 0.0f)
            {
                messageShowRatio = 0.0f;
                messageImageObject.SetActive(false);
            }
            else
            {
                messageImage.color = new Color(1.0f, 1.0f, 1.0f, messageShowRatio);
                messageText.color = new Color(1.0f, 0.7f, 0.0f, messageShowRatio);
            }
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

    public void ShowShopWindow(Shop shop)
    {
        shopWindow.SetActive(true);
        shopWindowC.ShowShop(shop);
    }

    public void CloseShopWindow()
    {
        shopWindow.SetActive(false);
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
                titleText.color = Color.cyan;
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

    public void ShowItemDrag(Item item)
    {
        itemDragOpen = true;
        itemDragContainerObject.SetActive(true);
        itemDragImage.sprite = itemImages.GetItemSprite(item.Name);
    }

    public void CloseItemDrag()
    {
        itemDragOpen = false;
        itemDragContainerObject.SetActive(false);
    }

    public void DisplayMessage(string message)
    {
        messageDuration = message.Length * 0.15f;
        messageShowing = true;
        messageImageObject.SetActive(true);
        messageText.text = message;
    }
}
