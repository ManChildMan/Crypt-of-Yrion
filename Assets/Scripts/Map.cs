using Pathfinding;
using UnityEngine;

// TO DO:

//   * Implement lava.
//   * Implement inanimate objects such as coffins, thrones, tables, etc and implement cover system.
//   * Implement spawning of items.


// HEURISTICS FOR FINAL GENERATOR PROCEDURE
//
// * Generate chasm data using CA. 
// * Generate and iterate rooms.
// * Fill, connect and smooth rooms.
// * Overlay room data on chasm data.
// * Find and write walls. No walls adjacent to chasms.

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
    private UnityEngine.Object m_floor;
    private UnityEngine.Object m_rubble;
    private UnityEngine.Object m_stoneBlock;
    private UnityEngine.Object m_water;
    private UnityEngine.Object m_torch;
    private UnityEngine.Object m_chest;
    private UnityEngine.Object m_stairwellUp;
    private UnityEngine.Object m_stairwellDown;
    float m_minX;
    float m_minZ;

    
	void Start ()
    {
        // Calculate the center of the first map element in world coordinates.
        m_minX = OriginX - ((Width * NodeSize) / 2) + (NodeSize / 2);
        m_minZ = OriginZ - ((Depth * NodeSize) / 2) + (NodeSize / 2);

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
            InstantiateMap(generator.MapData);
            InstantiateTorches(generator.ObjectData);
            InstantiateStairwells(generator.MapData);
            //InstantiateMap(CellularAutomaton.GetCellularAutomata());
            UserInterface.InitializeMiniMap(generator.MapData);
            
        }

        // Initialize our A* pathfinding graph.
        m_astarData = AstarPath.active.astarData;
        GridGraph graph = (GridGraph)m_astarData.AddGraph(typeof(GridGraph));
        graph.width = Width;
        graph.depth = Depth;
        graph.nodeSize = NodeSize;
        graph.center = new Vector3(OriginX, -10f, OriginZ);
        graph.UpdateSizeFromWidthDepth();
        graph.cutCorners = false;
        graph.collision.collisionCheck = true;
        graph.collision.mask = 1 << LayerMask.NameToLayer("Obstacle");
        graph.collision.heightCheck = true;
        graph.collision.heightMask = 1 << LayerMask.NameToLayer("Ground");
        AstarPath.active.Scan();

        // Overlay a grid of game objects to allow granular selection of 
        // the maps grid squares.
        //m_gridSquare = Resources.Load("GridSquare");
        //m_gridSquares = new Transform[Width, Depth];
        //float xPos = m_minX;
        //float zPos = m_minZ;
        //for (int x = 0; x < Width; x++)
        //{
        //    for (int z = 0; z < Depth; z++)
        //    {
        //        GameObject gridSquare = (GameObject)Instantiate(m_gridSquare);
        //        gridSquare.transform.parent = Grid;
        //        gridSquare.transform.position = new Vector3(xPos, 0, zPos);
        //        gridSquare.transform.localScale = 
        //            new Vector3(NodeSize, 0.02f, NodeSize);
        //        m_gridSquares[x, z] = gridSquare.transform;
        //        zPos += NodeSize;
        //    }
        //    zPos = m_minZ;
        //    xPos += NodeSize;
        //}

        // The user interface script can't register for map events until all 
        // grid square prefabs are created. Instruct the script to register for
        // events now.
        UserInterface.RegisterForMapEvents();
	}

    // Instantiates the appropriate prefab at each world space map location. 

    void InstantiateTorches(int[,] torchData)
    {
        m_torch = Resources.Load("WallTorch");
        m_chest = Resources.Load("Chest");
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                if (torchData[x, z] == (int)TerrainType.Torch_WallMounted_North)
                {
                    GameObject torch = (GameObject)Instantiate(m_torch);
                    torch.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize) + 1;
                    torch.transform.position = new Vector3(xPos, 1.2f, zPos - 0.35f);
                    torch.transform.rotation = Quaternion.Euler(0f, 140f, 0f);
                }
                else if (torchData[x, z] == (int)TerrainType.Torch_WallMounted_South)
                {
                    GameObject torch = (GameObject)Instantiate(m_torch);
                    torch.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize) - 1;
                    torch.transform.position = new Vector3(xPos, 1.2f, zPos + 0.35f);
                    torch.transform.rotation = Quaternion.Euler(0f, 140f + 180f, 0f);
                }
                else if (torchData[x, z] == (int)TerrainType.Torch_WallMounted_East)
                {
                    GameObject torch = (GameObject)Instantiate(m_torch);
                    torch.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    torch.transform.position = new Vector3(xPos - 0.65f, 1.2f, zPos);
                    torch.transform.rotation = Quaternion.Euler(0f, 140f - 90f, 0f);
                }
                else if (torchData[x, z] == (int)TerrainType.Torch_WallMounted_West)
                {
                    GameObject torch = (GameObject)Instantiate(m_torch);
                    torch.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    torch.transform.position = new Vector3(xPos + 0.65f, 1.2f, zPos);
                    torch.transform.rotation = Quaternion.Euler(0f, 140f + 90f, 0f);
                }
                else if (torchData[x, z] == (int)TerrainType.Item_Chest_North)
                {
                    GameObject chest = (GameObject)Instantiate(m_chest);
                    chest.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize) + 1;
                    chest.transform.position = new Vector3(xPos, 0, zPos);
                    chest.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else if (torchData[x, z] == (int)TerrainType.Item_Chest_South)
                {
                    GameObject chest = (GameObject)Instantiate(m_chest);
                    chest.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize) - 1;
                    chest.transform.position = new Vector3(xPos, 0, zPos);
                    chest.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
                else if (torchData[x, z] == (int)TerrainType.Item_Chest_East)
                {
                    GameObject chest = (GameObject)Instantiate(m_chest);
                    chest.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    chest.transform.position = new Vector3(xPos - 1, 0, zPos);
                    chest.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                }
                else if (torchData[x, z] == (int)TerrainType.Item_Chest_West)
                {
                    GameObject chest = (GameObject)Instantiate(m_chest);
                    chest.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    chest.transform.position = new Vector3(xPos + 1, 0, zPos);
                    chest.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                }
            }
        }
    }
    enum Facing
    {
        North = 1,
        South = 2,
        East = 3, 
        West = 4
    }
    void InstantiateStairwells(int[,] data)
    {
        m_stairwellUp = Resources.Load("StairwellUp");
        m_stairwellDown = Resources.Load("StairwellDown");
        float y = 4f;
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                if (data[x, z] == TerrainType.Stairs_UpNorth)
                {
                    GameObject stairwell = (GameObject)Instantiate(m_stairwellUp);
                    
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    stairwell.transform.position = new Vector3(xPos, y, zPos);

                }
                if (data[x, z] == TerrainType.Stairs_UpSouth)
                {
                    GameObject stairwell = (GameObject)Instantiate(m_stairwellUp);

                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    stairwell.transform.position = new Vector3(xPos, y, zPos);

                }
                if (data[x, z] == TerrainType.Stairs_UpEast)
                {
                    GameObject stairwell = (GameObject)Instantiate(m_stairwellUp);

                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    stairwell.transform.position = new Vector3(xPos, y, zPos);


                }
                if (data[x, z] == TerrainType.Stairs_UpWest)
                {
                    GameObject stairwell = (GameObject)Instantiate(m_stairwellUp);

                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    stairwell.transform.position = new Vector3(xPos, y, zPos);

                }
                if (data[x, z] == TerrainType.Stairs_DownNorth)
                {
                    GameObject stairwell = (GameObject)Instantiate(m_stairwellDown);

                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    stairwell.transform.position = new Vector3(xPos, y, zPos);

                }
                if (data[x, z] == TerrainType.Stairs_DownSouth)
                {
                    GameObject stairwell = (GameObject)Instantiate(m_stairwellDown);

                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    stairwell.transform.position = new Vector3(xPos, y, zPos);

                }
                if (data[x, z] == TerrainType.Stairs_DownEast)
                {

                    GameObject stairwell = (GameObject)Instantiate(m_stairwellDown);

                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    stairwell.transform.position = new Vector3(xPos, y, zPos);
                }
                if (data[x, z] == TerrainType.Stairs_DownWest)
                {
                    GameObject stairwell = (GameObject)Instantiate(m_stairwellDown);

                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    stairwell.transform.position = new Vector3(xPos, y, zPos);

                }
            }
        }
    }



    void InstantiateMap(int[,] data)
    {
        m_floor = Resources.Load("Floor");
        m_rubble = Resources.Load("Rubble");
        m_stoneBlock = Resources.Load("StoneBlock");
        m_water = Resources.Load("Water_Shallow");

        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                if (data[x, z] == (int)TerrainType.Chasm)
                {
         

                }
                else if (data[x, z] == (int)TerrainType.Floor_01)
                {
                    GameObject floor = (GameObject)Instantiate(m_floor);
                    floor.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    floor.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (data[x, z] == (int)TerrainType.Impassable_Rubble_01)
                {
                    // Floor
                    GameObject rubble = (GameObject)Instantiate(m_rubble);
                    rubble.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    rubble.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (data[x, z] == (int)TerrainType.Wall_Stone_01)
                {
                    // Stone Block                 
                    GameObject block = (GameObject)Instantiate(m_stoneBlock);
                    block.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    block.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (data[x, z] == (int)TerrainType.Water_Shallow)
                {
                    // Stone Block                 
                    GameObject water = (GameObject)Instantiate(m_water);
                    water.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    water.transform.position = new Vector3(xPos, 0, zPos);
                }
            }
        }

    }
}
