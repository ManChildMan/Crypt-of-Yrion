using UnityEngine;
using UnityEngine.UI;

// This script will be responsible for managing user input.
public class UserInterface : MonoBehaviour
{
    // Reference to the map object.
    private Map map;

    // Variables used for the minimap.
    private int[,] mapData;
    private Transform playerTransform;
    private Vector3 prevPlayerPosition;
    private Texture2D miniMapTexture;

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
        // Obtain required object references needed for initializing the minimap.
        GameObject miniMap = GameObject.Find("MiniMap");
        GameObject adventurer = GameObject.Find("Adventurer");
        map = GameObject.Find("Map").GetComponent<Map>();

        // Set variables which will be used when updating/rendering the minimap.
        mapData = data;
        playerTransform = adventurer.transform;

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
        if (playerPosition != prevPlayerPosition)
        {
            prevPlayerPosition = playerPosition;
            int startX = (int)(playerPosition.x + map.Width * map.NodeSize / 2.0f - miniMapTexture.width / 2.0f);
            int startY = (int)(playerPosition.z + map.Depth * map.NodeSize / 2.0f - miniMapTexture.height / 2.0f);
            for (int x = 0; x < miniMapTexture.width; x++)
            {
                for (int y = 0; y < miniMapTexture.height; y++)
                {
                    // Ensure element is inside the map, otherwise just draw the pixel as black.
                    if (startX + x < mapData.GetLength(0) && startX + x >= 0 && startY + y < mapData.GetLength(1) && startY + y >= 0)
                    {
                        switch (mapData[startX + x, startY + y])
                        {
                            case TerrainType.Floor_01:
                                miniMapTexture.SetPixel(x, y, Color.white);
                                break;
                            case TerrainType.Wall_Stone_01:
                                miniMapTexture.SetPixel(x, y, Color.gray);
                                break;
                            case TerrainType.Water_Deep:
                                miniMapTexture.SetPixel(x, y, Color.blue);
                                break;
                            case TerrainType.Water_Shallow:
                                miniMapTexture.SetPixel(x, y, new Color(0.0f, 0.0f, 0.6f));
                                break;
                            default:
                                miniMapTexture.SetPixel(x, y, Color.black);
                                break;
                        }
                    }
                    else
                    {
                        miniMapTexture.SetPixel(x, y, Color.black);
                    }
                }
            }
            miniMapTexture.Apply();
        }
    }
}
