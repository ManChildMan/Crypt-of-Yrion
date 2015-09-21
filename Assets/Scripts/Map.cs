using Pathfinding;
using UnityEngine;

// This script creates a grid of GridSquare game objects to show the locations
// of the maps grid squares and enable grid square selection via the mouse. If 
// GenerateMap is true a random map is generated.
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


    private UnityEngine.Object m_mapSquare;
    private Transform[,] m_map;
    private AstarData m_astarData;
    private UnityEngine.Object m_rubble;
    private UnityEngine.Object m_stoneBlock;
    float m_minX;
    float m_minZ;

    
	void Start ()
    {
        m_map = new Transform[Width, Depth];
        m_mapSquare = Resources.Load("MapSquare");

        // Calculate the center of the first map element in world coordinates.
        m_minX = OriginX - ((Width * NodeSize) / 2) + (NodeSize / 2);
        m_minZ = OriginZ - ((Depth * NodeSize) / 2) + (NodeSize / 2);

        // Scale and position the ground plane according to map width, map
        // depth and origin.
        // To do: Scale texture coordinates so texture is not distorted at
        // different map sizes.
        Ground.localScale = new Vector3(
            ((float)Width * NodeSize) / 10, 1, ((float)Depth * NodeSize) / 10);
        Ground.transform.position = new Vector3(OriginX, 0, OriginZ);

        m_rubble = Resources.Load("Rubble");
        m_stoneBlock = Resources.Load("StoneBlock");

        if (GenerateMap)
        {
            MapGenerator gen = new MapGenerator(Width, Depth, RandomSeed);
            gen.RoomCount = RoomCount;
            gen.SpawnDistance = SpawnDistance;
            gen.MinRoomSideRatio = MinRoomSideRatio;
            gen.SeparationStrength = SeparationStrength;
            gen.MinRoomWidth = MinRoomWidth;
            gen.MaxRoomWidth = MaxRoomWidth;
            gen.MinRoomHeight = MinRoomHeight;
            gen.MaxRoomHeight = MaxRoomHeight;
            gen.UseSmoothing = UseSmoothing;
            gen.SmoothingThreshold = SmoothingThreshold;
            gen.SmoothingIterations = SmoothingIterations;
            gen.WeightedGraphEdgeRadius = WeightedGraphEdgeRadius;
            gen.Generate();
            InstantiateMap(gen.Data);
        }

        // Initialise A* pathfinding graph.
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
        // To do: Optimize by not creating grid square game objects in terrain
        // squares.
        float xPos = m_minX;
        float zPos = m_minZ;
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                GameObject gridSquare = (GameObject)Instantiate(m_mapSquare);
                gridSquare.transform.parent = Grid;
                gridSquare.transform.position = new Vector3(xPos, 0, zPos);
                gridSquare.transform.localScale = 
                    new Vector3(NodeSize, 0.02f, NodeSize);
                m_map[x, z] = gridSquare.transform;
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

    // Instantiates the appropriate prefab at each map location. 
    void InstantiateMap(int[,] data)
    {    
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
