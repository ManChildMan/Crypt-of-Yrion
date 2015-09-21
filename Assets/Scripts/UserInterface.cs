using UnityEngine;

// This script will be responsible for managing user input.
public class UserInterface : MonoBehaviour
{
    public GridSquare SelectedGridSquare
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
    
    private GridSquare m_selectedTile;

    void Start()
    {

    }
    public void RegisterForMapEvents()
    {
        GridSquare[] squares = FindObjectsOfType<GridSquare>();
        foreach (GridSquare square in squares)
        {
            square.OnGridSquareSelected += MapSquareSelectedHandler;
        }
    }

    public void OnDisable()
    {
        GridSquare[] squares = FindObjectsOfType<GridSquare>();
        foreach (GridSquare square in squares)
        {
            square.OnGridSquareSelected -= MapSquareSelectedHandler;
        }
    }

    void MapSquareSelectedHandler(GridSquare square)
    {        
        if (SelectedGridSquare != null)
        {
            SelectedGridSquare.Unselect();
        }
        SelectedGridSquare = square;
    }
}
