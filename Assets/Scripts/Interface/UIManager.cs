using UnityEngine;
using UnityEngine.UI;

// Manages UI on the canvas game object (includes input on the canvas, use UserInterface for world input).
public class UIManager : MonoBehaviour
{
    public Inventory inventory;
    public ItemImages itemImages;

    public GameObject transitionImageObject;
    private Image transitionImage;
    private float transition = 1.0f;

    public GameObject inventoryWindow;
    public GameObject statsWindow;
    public GameObject equipmentWindow;
    public GameObject shopWindow;
    private ShopWindow shopWindowC;
    public GameObject lootWindow;
    private LootWindow lootWindowC;
    public GameObject itemPreviewWindow;
    private bool itemPreviewWindowOpen;

    public GameObject portalDialog;
    private Text portalDialogText;
    private PortalAction portalAction;
    private bool portalTransition;

    public GameObject itemDragContainerObject;
    private Image itemDragImage;
    private bool itemDragOpen;

    public GameObject messageImageObject;
    private Image messageImage;
    private Text messageText;
    private bool messageShowing;
    private float messageDuration;
    private float messageShowRatio;

    public GameObject loadDialog;
    private CanvasGroup loadDialogCanvasGroup;

    private Animator Ani;
    private int PlayerHealth;
    private float restartTimer;
    private float restartDelay = 5f;

    void Start () {
        transitionImage = transitionImageObject.GetComponent<Image>();
        transitionImage.color = new Color(0.0f, 0.0f, 0.0f, 1f);
        transitionImageObject.SetActive(true);

        shopWindowC = shopWindow.GetComponent<ShopWindow>();
        lootWindowC = lootWindow.GetComponent<LootWindow>();

        portalDialogText = portalDialog.transform.FindChild("Text").gameObject.GetComponent<Text>();

        itemDragImage = itemDragContainerObject.transform.FindChild("ItemDragImage").gameObject.GetComponent<Image>();

        messageImage = messageImageObject.GetComponent<Image>();
        messageImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        messageText = messageImageObject.transform.FindChild("MessageText").gameObject.GetComponent<Text>();
        messageText.color = new Color(1.0f, 0.7f, 0.0f, 0.0f);

        loadDialogCanvasGroup = loadDialog.GetComponent<CanvasGroup>();

        PlayerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().CurrentHealth;
    }

    void Update()
    {
        PlayerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().CurrentHealth;

        if (PlayerHealth <= 0)
        {
            // ... tell the animator the game is over.
            Ani.SetTrigger("GameOver");

            // .. increment a timer to count up to restarting.
            restartTimer += Time.deltaTime;

            // .. if it reaches the restart delay...
            if (restartTimer >= restartDelay)
            {
                // .. then reload the currently loaded level.
                Application.LoadLevel("safezone");
            }
        }

        // Update transitons.
        if (!portalTransition)
        {
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
        }
        else
        {
            transition += Time.deltaTime;
            if (transition >= 1.0f)
            {
                loadDialogCanvasGroup.alpha = 1.0f;
                transitionImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                UsePortalNow();
                return;
            }
            else
            {
                loadDialogCanvasGroup.alpha = transition;
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

    public void ShowLootWindow(Loot loot)
    {
        lootWindow.SetActive(true);
        lootWindowC.ShowLoot(loot);
    }

    public void CloseLootWindow()
    {
        lootWindow.SetActive(false);
    }

    public void ShowPortalDialog(string text, PortalAction portalAction)
    {
        portalDialog.SetActive(true);
        portalDialogText.text = text;
        this.portalAction = portalAction;
    }

    public void ClosePortalDialog()
    {
        portalDialog.SetActive(false);
    }

    public void UsePortal()
    {
        transitionImageObject.SetActive(true);
        transitionImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        portalTransition = true;
        loadDialog.SetActive(true);
    }

    public void UsePortalNow()
    {
        StateMigrator.lastPortalActionTaken = portalAction;
        inventory.SaveState();
        switch (portalAction)
        {
            case PortalAction.ExitLevel1:
            case PortalAction.ExitLevel2:
            case PortalAction.ExitLevel3:
                Application.LoadLevel("safezone");
                break;
            case PortalAction.GotoLevel1:
                Application.LoadLevel("level1");
                break;
            case PortalAction.GotoLevel2:
                Application.LoadLevel("level2");
                break;
            case PortalAction.GotoLevel3:
                Application.LoadLevel("level3");
                break;
        }
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
            else
            {
                healthTextObject.SetActive(false);
            }
            if (gearItem.Speed != 0)
            {
                speedTextObject.gameObject.SetActive(true);
                speedTextObject.GetComponent<Text>().text = string.Format("+{0:n0} Speed", gearItem.Speed);
            }
            else
            {
                speedTextObject.SetActive(false);
            }
            if (gearItem.Attack != 0)
            {
                attackTextObject.gameObject.SetActive(true);
                attackTextObject.GetComponent<Text>().text = string.Format("+{0:n0} Attack", gearItem.Attack);
            }
            else
            {
                attackTextObject.SetActive(false);
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
