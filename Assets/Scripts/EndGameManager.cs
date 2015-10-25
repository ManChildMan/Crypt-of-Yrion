using UnityEngine;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Text lastMessage;
    public GameObject menuButtonObject;

    private float showRatio;
    private string text;

    private float indexCounterTimer;
    private int index;

    void Start()
    {
        text = string.Format("You have defeated the corrupted physical form of Yrion... As you see the knight fall to the ground there is faint glow... The spirit that was contained within the knight has been freed." +
            "\n\nYou completed the game in {0} seconds.\nYou died {1} times.\n\nDeveloped by: Nathaniel Fuller, Jordan Singh and James Manning.\n\nThank you for playing.", (int)StateMigrator.gameTimer, StateMigrator.deathCounter);
    }

    void Update()
    {
        if (showRatio != 1.0f)
        {
            showRatio += Time.deltaTime;
            if (showRatio >= 1.0f)
            {
                showRatio = 1.0f;
            }
            canvasGroup.alpha = showRatio;
        }
        else
        {
            if (index != text.Length)
            {
                indexCounterTimer += Time.deltaTime;
                while (indexCounterTimer > 0.05f && index < text.Length)
                {
                    indexCounterTimer -= 0.05f;
                    index++;
                }
                lastMessage.text = text.Substring(0, index);
                if (index == text.Length)
                {
                    menuButtonObject.SetActive(true);
                }
            }
        }
    }

    public void BackToMenu()
    {
        StateMigrator.lastPortalActionTaken = PortalAction.None;
        StateMigrator.allItems = null;
        StateMigrator.wealth = 0;
        StateMigrator.anyWindowOpen = false;
        StateMigrator.deathCounter = 0;
        StateMigrator.gameTimer = 0;
        Application.LoadLevel("menu");
    }
}
