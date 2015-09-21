using UnityEngine;

public delegate void MapSquareSelected(MapSquare square);

// This script is attached to the GridSquare prefab which is responsible for
// displaying grid squares and intercepting mouse clicks.
public class MapSquare : MonoBehaviour
{

    public event MapSquareSelected OnMapSquareSelected;
    public bool IsSelected = false;
    public Material UnselectedMaterial;
    public Material SelectedMaterial;

    private Renderer m_renderer;    
    
    void Start()
    {
        m_renderer = GetComponentInChildren<MeshRenderer>();
	}

	void Update ()
    {
	}

    void OnMouseUp()
    {
        // Return if this is selected grid square.
        if (IsSelected)
            return;

        m_renderer.material = SelectedMaterial;
        IsSelected = true;

        if (OnMapSquareSelected != null)
        {
            OnMapSquareSelected(this);
        }
    }

    public void Unselect()
    {
        m_renderer.material = UnselectedMaterial;
        IsSelected = false;
    }
}
