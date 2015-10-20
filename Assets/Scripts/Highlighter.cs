using UnityEngine;

public class Highlighter : MonoBehaviour
{
    public string message;
    private bool showMessage;

    void OnMouseOver()
    {
        showMessage = true;
    }

    void OnMouseExit()
    {
        showMessage = false;
    }

    void OnGUI()
    {
        if (showMessage)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x - 155, Event.current.mousePosition.y, 150, 25), message);
        }
    }
}
