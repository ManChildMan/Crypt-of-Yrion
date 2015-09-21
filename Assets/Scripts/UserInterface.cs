using UnityEngine;

// This script will be responsible for managing user input.
public class UserInterface : MonoBehaviour
{
    public MapSquare SelectedGridSquare
    {
        get
        {
            return m_selectedTile;
        }
        set
        {
            m_selectedTile = value;
        }
    }
    
    private MapSquare m_selectedTile;

    void Start()
    {

    }
    public void RegisterForMapEvents()
    {
        MapSquare[] squares = FindObjectsOfType<MapSquare>();
        foreach (MapSquare square in squares)
        {
            square.OnMapSquareSelected += MapSquareSelectedHandler;
        }
    }

    public void OnDisable()
    {
        MapSquare[] squares = FindObjectsOfType<MapSquare>();
        foreach (MapSquare square in squares)
        {
            square.OnMapSquareSelected -= MapSquareSelectedHandler;
        }
    }

    void MapSquareSelectedHandler(MapSquare square)
    {        
        if (SelectedGridSquare != null)
        {
            SelectedGridSquare.Unselect();
        }
        SelectedGridSquare = square;
    }
}
