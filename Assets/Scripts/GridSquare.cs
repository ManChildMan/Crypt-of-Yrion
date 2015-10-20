using UnityEngine;

public delegate void GridSquareSelected(GridSquare square);

/// <summary>
/// This script is attached to the GridSquare prefab. It is responsible for 
/// changing the material applied to a grid square when it is selected or
/// unselected. UI code needs to subscribe to the OnGridSquareSelected event
/// for each GridSquare to receive selected notifications.
/// </summary>
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

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            // Return if already selected.
            if (IsSelected)
                return;

            // Set the 'selected' material.
            m_renderer.material = SelectedMaterial;
            IsSelected = true;

            // Fire the selected event.
            if (OnGridSquareSelected != null)
                OnGridSquareSelected(this);
        }
    }

    public void Unselect()
    {
        // Set the 'unselected' material.
        m_renderer.material = UnselectedMaterial;
        IsSelected = false;
    }
}
