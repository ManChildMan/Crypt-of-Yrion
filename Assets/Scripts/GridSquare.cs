using UnityEngine;

public delegate void GridSquareSelected(GridSquare square);

// This script is attached to the GridSquare prefab which is responsible for
// displaying grid squares and intercepting mouse clicks.
public class GridSquare : MonoBehaviour
{

    public event GridSquareSelected OnGridSquareSelected;
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
        if (IsSelected)
            return;

        m_renderer.material = SelectedMaterial;
        IsSelected = true;

        if (OnGridSquareSelected != null)
        {
            OnGridSquareSelected(this);
        }
    }

    public void Unselect()
    {
        m_renderer.material = UnselectedMaterial;
        IsSelected = false;
    }
}
