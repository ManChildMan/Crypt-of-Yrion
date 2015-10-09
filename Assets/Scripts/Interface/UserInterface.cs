using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is will be responsible for managing user input within the world.
/// </summary>
public class UserInterface : MonoBehaviour
{
    // References set by unity.
    public Map map;
    public GameObject miniMap;
    public Transform playerTransform;

    // Variables used for the minimap.
    private int[,] mapData;
    private Vector3 prevPlayerPosition;
    private Texture2D miniMapTexture;

    private int prevStartX;
    private int prevStartZ;

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

    public void InitializeMiniMap(int[,] data)
    {
        // Set variables which will be used when updating/rendering the minimap.
        mapData = data;

        // Create minimap texture and apply it to the UI image.
        Rect miniMapRect = miniMap.GetComponent<RectTransform>().rect;
        miniMapTexture = new Texture2D((int)miniMapRect.width, (int)miniMapRect.height);
        miniMapTexture.filterMode = FilterMode.Point;
        miniMap.GetComponent<Image>().sprite = Sprite.Create(miniMapTexture, new Rect(0, 0, miniMapTexture.width, miniMapTexture.height), Vector2.zero);
    }


    public void Update()
    {
        Vector3 playerPosition = playerTransform.transform.position;

        // Only redraw the minimap if the player moved.
        int startX = (int)(playerPosition.x + map.Width * map.NodeSize / 2.0f - miniMapTexture.width / 2.0f);
        int startZ = (int)(playerPosition.z + map.Depth * map.NodeSize / 2.0f - miniMapTexture.height / 2.0f);

        if (startX != prevStartX || startZ != prevStartZ)
        {
            prevStartX = startX;
            prevStartZ = startZ;
            for (int x = 0; x < miniMapTexture.width; x++)
            {
                for (int z = 0; z < miniMapTexture.height; z++)
                {
                    // Ensure element is inside the map, otherwise just draw the pixel as black.
                    if (startX + x < mapData.GetLength(0) && startX + x >= 0 && startZ + z < mapData.GetLength(1) && startZ + z >= 0)
                    {
                        switch (mapData[startX + x, startZ + z])
                        {
                            case TerrainType.Floor_01:
                                miniMapTexture.SetPixel(x, z, Color.white);
                                break;
                            case TerrainType.Wall_Stone_01:
                                miniMapTexture.SetPixel(x, z, Color.gray);
                                break;
                            case TerrainType.Water_Deep:
                                miniMapTexture.SetPixel(x, z, Color.blue);
                                break;
                            case TerrainType.Water_Shallow:
                                miniMapTexture.SetPixel(x, z, new Color(0.0f, 0.0f, 0.6f));
                                break;
                            default:
                                miniMapTexture.SetPixel(x, z, Color.black);
                                break;
                        }
                    }
                    else
                    {
                        miniMapTexture.SetPixel(x, z, Color.black);
                    }
                }
            }
            miniMapTexture.Apply();
        }
    }
}
