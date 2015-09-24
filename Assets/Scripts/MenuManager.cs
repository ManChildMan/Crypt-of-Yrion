using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
    
    // New Game button handler
    public void NewGame()
    {
        Application.LoadLevel("prototype");
    }

    // Load Game Button handler
    public void LoadGame()
    {

    }

    // Settings button handler
    public void Settings()
    {

    }

    // Quit button handler
    public void Quit()
    {
        Application.Quit();
    }
}
