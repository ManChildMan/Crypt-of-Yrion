using UnityEngine;
using System.Collections;

public class ObjectHighlight : MonoBehaviour {

    public string objectName;

    private Color startColour;
    private bool _displayObjectName;

    void OnGUI()
    {
        DisplayName();
    }

    void OnMouseEnter()
    {
        startColour = GetComponentInChildren<Renderer>().material.color;
        GetComponentInChildren<Renderer>().material.color = Color.blue;
        _displayObjectName = true;
    }

    void OnMouseExit()
    {
        GetComponentInChildren<Renderer>().material.color = startColour;
        _displayObjectName = false;
    }

    public void DisplayName()
    {
        if (_displayObjectName == true)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x - 155, Event.current.mousePosition.y, 150, 25), objectName);
        }
    }
}
