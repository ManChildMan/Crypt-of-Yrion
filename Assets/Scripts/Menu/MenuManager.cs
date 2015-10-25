using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // References set by unity.
    public GameObject transitionImageObject;
    private Image transitionImage;

    private bool transition;
    private ToState toState;
    private bool prevTransition;
    private float transitionRatio;

    private enum ToState
    {
        NewGame,
        Quit
    }

    void Start()
    {
        transitionImage = transitionImageObject.GetComponent<Image>();
    }

    void Update()
    {
        if (prevTransition != transition)
        {
            prevTransition = transition;
            transitionImageObject.SetActive(true);
            transitionImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        if (transition)
        {
            transitionRatio += Time.deltaTime;
            if (transitionRatio >= 1.0f)
            {
                switch (toState)
                {
                    case ToState.NewGame:
                        Application.LoadLevel("intro");
                        break;
                    case ToState.Quit:
                        Application.Quit();
                        // Application.Quit() does not work when running from Unity Editor, so just reshow everything after the transition effect.
                        transition = false;
                        prevTransition = false;
                        transitionImageObject.SetActive(false);
                        transitionRatio = 0.0f;
                        break;
                }
            }
            else
            {
                transitionImage.color = new Color(0.0f, 0.0f, 0.0f, transitionRatio);
            }
        }

    }
    
    public void NewGame()
    {
        if (transition)
        {
            return;
        }
        toState = ToState.NewGame;
        transition = true;
    }
    
    public void Quit()
    {
        toState = ToState.Quit;
        transition = true;
    }
}
