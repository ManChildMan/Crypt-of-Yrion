using Pathfinding;
using UnityEngine;

// TO DO:
//   * Scale texture coordinates so texture is not distorted at different map sizes.
//   * Optimize by not creating grid square objects in terrain grid squares.

/// <summary>
/// 
/// </summary>
public class Map : MonoBehaviour 
{
    public int Width = 50;
    public int Depth = 50;
    public int OriginX = 0;
    public int OriginZ = 0;
    public float NodeSize = 1;
    public Transform Ground;
    public Transform Obstacles;
    public Transform Grid;
    public UserInterface UserInterface;

    public bool GenerateMap = false;
    public int RandomSeed = 1;
    public int MinRoomWidth = 3;
    public int MaxRoomWidth = 18;
    public int MinRoomHeight = 3;
    public int MaxRoomHeight = 18;
    public float MinRoomSideRatio = 0.3f;
    public int RoomCount = 50;
    public int SpawnDistance = 20;
    public float SeparationStrength = 8f;
    public bool UseSmoothing = true;
    public int SmoothingThreshold = 4;
    public int SmoothingIterations = 2;
    public float WeightedGraphEdgeRadius = 50f;


    private UnityEngine.Object m_gridSquare;
    private Transform[,] m_gridSquares;
    private AstarData m_astarData;
    private UnityEngine.Object m_rubble;
    private UnityEngine.Object m_stoneBlock;
    float m_minX;
    float m_minZ;

    
	void Start ()
    {
        // Calculate the center of the first map element in world coordinates.
        m_minX = OriginX - ((Width * NodeSize) / 2) + (NodeSize / 2);
        m_minZ = OriginZ - ((Depth * NodeSize) / 2) + (NodeSize / 2);

        // Scale and position the ground plane according to map width, depth 
        // and origin.
        Ground.localScale = new Vector3(((float)Width * NodeSize) / 10, 1, 
            ((float)Depth * NodeSize) / 10);
        Ground.transform.position = new Vector3(OriginX, 0, OriginZ);

        if (GenerateMap)
        {
            MapGenerator generator = new MapGenerator(Width, Depth, RandomSeed);
            generator.RoomCount = RoomCount;
            generator.SpawnDistance = SpawnDistance;
            generator.MinRoomSideRatio = MinRoomSideRatio;
            generator.SeparationStrength = SeparationStrength;
            generator.MinRoomWidth = MinRoomWidth;
            generator.MaxRoomWidth = MaxRoomWidth;
            generator.MinRoomHeight = MinRoomHeight;
            generator.MaxRoomHeight = MaxRoomHeight;
            generator.UseSmoothing = UseSmoothing;
            generator.SmoothingThreshold = SmoothingThreshold;
            generator.SmoothingIterations = SmoothingIterations;
            generator.WeightedGraphEdgeRadius = WeightedGraphEdgeRadius;
            generator.Generate();
            // The map generator works with an array of integers. The following
            // function converts this array to game objects in the scene.
            InstantiateMap(generator.Data);
        }

        // Initialize our A* pathfinding graph.
        m_astarData = AstarPath.active.astarData;
        GridGraph graph = (GridGraph)m_astarData.AddGraph(typeof(GridGraph));
        graph.width = Width;
        graph.depth = Depth;
        graph.nodeSize = NodeSize;
        graph.center = new Vector3(OriginX, -0.1f, OriginZ);
        graph.UpdateSizeFromWidthDepth();
        graph.cutCorners = false;
        graph.collision.collisionCheck = true;
        graph.collision.mask = 1 << LayerMask.NameToLayer("Obstacle");
        graph.collision.heightCheck = true;
        graph.collision.heightMask = 1 << LayerMask.NameToLayer("Ground");
        AstarPath.active.Scan();

        // Overlay a grid of game objects to allow granular selection of 
        // the maps grid squares.
        m_gridSquare = Resources.Load("GridSquare");
        m_gridSquares = new Transform[Width, Depth];
        float xPos = m_minX;
        float zPos = m_minZ;
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                GameObject gridSquare = (GameObject)Instantiate(m_gridSquare);
                gridSquare.transform.parent = Grid;
                gridSquare.transform.position = new Vector3(xPos, 0, zPos);
                gridSquare.transform.localScale = 
                    new Vector3(NodeSize, 0.02f, NodeSize);
                m_gridSquares[x, z] = gridSquare.transform;
                zPos += NodeSize;
            }
            zPos = m_minZ;
            xPos += NodeSize;
        }

        // The user interface script can't register for map events until all 
        // grid square prefabs are created. Instruct the script to register for
        // events now.
        UserInterface.RegisterForMapEvents();
	}

    // Instantiates the appropriate prefab at each world space map location. 
    void InstantiateMap(int[,] data)
    {
        m_rubble = Resources.Load("Rubble");
        m_stoneBlock = Resources.Load("StoneBlock");
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                if (data[x, z] == -1)
                {
                    // Rubble
                    GameObject rubble = (GameObject)Instantiate(m_rubble);
                    rubble.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    rubble.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (data[x, z] == 0)
                {
                    // Empty
                }
                else if (data[x, z] == 1)
                {
                    // Stone Block                 
                    GameObject block = (GameObject)Instantiate(m_stoneBlock);
                    block.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    block.transform.position = new Vector3(xPos, 0, zPos);
                }
            }
        }
    }
}
