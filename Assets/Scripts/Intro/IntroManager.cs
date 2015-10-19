using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class IntroManager : MonoBehaviour
{
    // References set by unity.
    public GameObject textObject;
    public GameObject nextButtonObject;
    public string[] strings;

    private Text text;
    private Text nextButtonText;

    private int index;
    private int indexWithinString;
    private bool showing;
    private const float showLetterTimerTotal = 0.05f; // Show a letter every 1/10th of a second (100 milliseconds).
    private float showLetterTimer; 

	void Start ()
    {
        text = textObject.GetComponent<Text>();
        nextButtonText = nextButtonObject.transform.FindChild("Text").gameObject.GetComponent<Text>();
        showing = true;
    }
	
	void Update ()
    {
        if (showing)
        {
            showLetterTimer += Time.deltaTime;
            while (showLetterTimer >= showLetterTimerTotal)
            {
                showLetterTimer -= showLetterTimerTotal;
                indexWithinString++;
                text.text = strings[index].Substring(0, indexWithinString);
                if (indexWithinString == strings[index].Length)
                {
                    showing = false;
                    nextButtonObject.SetActive(true);
                    if (index == strings.Length - 1)
                    {
                        nextButtonText.text = "Enter the Crypt";
                    }
                }
            }
        }
	}

    public void NextLine()
    {
        if (index < strings.Length - 1)
        {
            showLetterTimer = 0.0f;
            text.text = "";
            showing = true;
            index++;
            indexWithinString = 0;
            nextButtonObject.SetActive(false);
        }
        else
        {
            Application.LoadLevel("safezone");
        }
    }
}
