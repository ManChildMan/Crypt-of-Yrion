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
    public MapCrafter MapCrafter;

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
    
    private AstarData m_astarData;
    private UnityEngine.Object m_floor;
    private UnityEngine.Object m_rubble;
    private UnityEngine.Object m_stoneBlock;
    private UnityEngine.Object m_water;
    private UnityEngine.Object m_pillar1;
    private UnityEngine.Object m_pillar2;
    private UnityEngine.Object m_pillar3;
    private UnityEngine.Object m_torch;
    private UnityEngine.Object m_chest;
    private UnityEngine.Object m_stairwellUp;
    private UnityEngine.Object m_stairwellDown;
    private float m_minX;
    private float m_minZ;

    // These two arrays should be the same size and set by the unity editor.
    public GameObject[] enemyPrefabs;
    public int[] enemyPrefabsSpawnCount;
    
	void Start ()
    {
        // Calculate the center of the first map element (i.e. (0, 0)) in 
        // world space coordinates.
        m_minX = OriginX - ((Width * NodeSize) / 2) + (NodeSize / 2);
        m_minZ = OriginZ - ((Depth * NodeSize) / 2) + (NodeSize / 2);

        int[,] mapData;
        int[,] objectData;
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
            mapData = generator.MapData;
            objectData = generator.ObjectData;
        }
        else
        {
            mapData = MapCrafter.GetMapData(this);
            objectData = MapCrafter.GetPropData(this);
        }

        // The map generator works with 2D arrays of integers. The following
        // functions convert the output arrays into actual game objects in 
        // the scene.
        InstantiateMap(mapData);
        InstantiateProps(objectData);
        InstantiateStairwells(mapData);

        // The user interface script can't register for map events until all 
        // grid square prefabs are created. Instruct the script to register for
        // events now. We can also initialise the minimap at this stage.
        UserInterface.RegisterForMapEvents();
        UserInterface.InitializeMiniMap(mapData);

        // Initialize a A* pathfinding graph.
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

        // Spawn enemies on the map.
        SpawnEnemies(mapData);
    }

    // Instantiates the appropriate prefab at each world space map location. 

    void InstantiateProps(int[,] propData)
    {
        m_torch = Resources.Load("WallTorch");
        m_chest = Resources.Load("Chest");

        // Ensure the chests can talk with the game (for accessing loot).
        GameObject m_chestAccessor = (GameObject)Resources.Load("ChestLootAccessor");
        ChestLootAccessor m_chestLootAccessor = m_chestAccessor.GetComponent<ChestLootAccessor>();
        GameObject uiAndInventory = GameObject.Find("UIAndInventory");
        m_chestLootAccessor.inventory = uiAndInventory.transform.FindChild("Inventory").GetComponent<Inventory>();
        m_chestLootAccessor.uiManager = uiAndInventory.transform.FindChild("Canvas").GetComponent<UIManager>();
        m_chestLootAccessor.playerController = GameObject.Find("Adventurer").GetComponent<PlayerController>();

        GameObject instance = null;
        float xPos = 0, zPos = 0;
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                int element = propData[x, z];
                if (element == TerrainType.Torch_WallMounted_North)
                {
                    instance = (GameObject)Instantiate(m_torch);
                    instance.transform.parent = Obstacles;
                    xPos = m_minX + (x * NodeSize);
                    zPos = m_minZ + (z * NodeSize) + 1;
                    instance.transform.position = new Vector3(xPos, 1.2f, zPos - 0.35f);
                    instance.transform.rotation = Quaternion.Euler(0f, 180f - 45f, 0f);
                }
                else if (element == TerrainType.Torch_WallMounted_South)
                {
                    instance = (GameObject)Instantiate(m_torch);
                    instance.transform.parent = Obstacles;
                    xPos = m_minX + (x * NodeSize);
                    zPos = m_minZ + (z * NodeSize) - 1;
                    instance.transform.position = new Vector3(xPos, 1.2f, zPos + 0.35f);
                    instance.transform.rotation = Quaternion.Euler(0f, 0f - 45f, 0f);
                }
                else if (element == TerrainType.Torch_WallMounted_East)
                {
                    instance = (GameObject)Instantiate(m_torch);
                    instance.transform.parent = Obstacles;
                    xPos = m_minX + (x * NodeSize);
                    zPos = m_minZ + (z * NodeSize);
                    instance.transform.position = new Vector3(xPos + 0.65f, 1.2f, zPos);
                    instance.transform.rotation = Quaternion.Euler(0f, 270f - 45f, 0f);
                }
                else if (element == TerrainType.Torch_WallMounted_West)
                {
                    instance = (GameObject)Instantiate(m_torch);
                    instance.transform.parent = Obstacles;
                    xPos = m_minX + (x * NodeSize);
                    zPos = m_minZ + (z * NodeSize);
                    instance.transform.position = new Vector3(xPos - 0.65f, 1.2f, zPos);
                    instance.transform.rotation = Quaternion.Euler(0f, 90f - 45f, 0f);
                }



                else if (propData[x, z] == (int)TerrainType.Item_Chest_North)
                {
                    GameObject chest = (GameObject)Instantiate(m_chest);
                    chest.transform.parent = Obstacles;
                    xPos = m_minX + (x * NodeSize);
                    zPos = m_minZ + (z * NodeSize) + 1;
                    chest.transform.position = new Vector3(xPos, 0, zPos);
                    chest.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

                    GameObject chestAccessor = Instantiate(m_chestAccessor);
                    chest.transform.parent = Obstacles;
                    chestAccessor.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (propData[x, z] == (int)TerrainType.Item_Chest_South)
                {
                    GameObject chest = (GameObject)Instantiate(m_chest);
                    chest.transform.parent = Obstacles;
                    xPos = m_minX + (x * NodeSize);
                    zPos = m_minZ + (z * NodeSize) - 1;
                    chest.transform.position = new Vector3(xPos, 0, zPos);
                    chest.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                    GameObject chestAccessor = Instantiate(m_chestAccessor);
                    chest.transform.parent = Obstacles;
                    chestAccessor.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (propData[x, z] == (int)TerrainType.Item_Chest_East)
                {
                    GameObject chest = (GameObject)Instantiate(m_chest);
                    chest.transform.parent = Obstacles;
                    xPos = m_minX + (x * NodeSize);
                    zPos = m_minZ + (z * NodeSize);
                    chest.transform.position = new Vector3(xPos - 1, 0, zPos);
                    chest.transform.rotation = Quaternion.Euler(0f, -90f, 0f);

                    GameObject chestAccessor = Instantiate(m_chestAccessor);
                    chest.transform.parent = Obstacles;
                    chestAccessor.transform.position = new Vector3(xPos - 1, 0, zPos);
                }
                else if (propData[x, z] == (int)TerrainType.Item_Chest_West)
                {
                    GameObject chest = (GameObject)Instantiate(m_chest);
                    chest.transform.parent = Obstacles;
                    xPos = m_minX + (x * NodeSize);
                    zPos = m_minZ + (z * NodeSize);
                    chest.transform.position = new Vector3(xPos + 1, 0, zPos);
                    chest.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

                    GameObject chestAccessor = Instantiate(m_chestAccessor);
                    chest.transform.parent = Obstacles;
                    chestAccessor.transform.position = new Vector3(xPos + 1, 0, zPos);
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
        m_pillar1 = Resources.Load("Pillar1");
        m_pillar2 = Resources.Load("Pillar2");
        m_pillar3 = Resources.Load("Pillar3");

        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                if (data[x, z] == TerrainType.Chasm)
                {
         

                }
                else if (data[x, z] == TerrainType.Floor_01)
                {
                    GameObject floor = (GameObject)Instantiate(m_floor);
                    floor.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    floor.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (data[x, z] == TerrainType.Impassable_Rubble_01)
                {
                    // Floor
                    GameObject rubble = (GameObject)Instantiate(m_rubble);
                    rubble.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    rubble.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (data[x, z] == TerrainType.Wall_Stone_01)
                {
                    // Stone Block                 
                    GameObject block = (GameObject)Instantiate(m_stoneBlock);
                    block.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    block.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (data[x, z] == TerrainType.Water_Shallow)
                {
                    // Stone Block                 
                    GameObject water = (GameObject)Instantiate(m_water);
                    water.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    water.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (data[x, z] == TerrainType.Pillar_01)
                {                
                    GameObject water = (GameObject)Instantiate(m_pillar1);
                    water.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    water.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (data[x, z] == TerrainType.Pillar_02)
                {                
                    GameObject water = (GameObject)Instantiate(m_pillar2);
                    water.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    water.transform.position = new Vector3(xPos, 0, zPos);
                }
                else if (data[x, z] == TerrainType.Pillar_03)
                {               
                    GameObject water = (GameObject)Instantiate(m_pillar3);
                    water.transform.parent = Obstacles;
                    float xPos = m_minX + (x * NodeSize);
                    float zPos = m_minZ + (z * NodeSize);
                    water.transform.position = new Vector3(xPos, 0, zPos);
                }
            }
        }
    }

    void SpawnEnemies(int[,] mapData)
    {
        if (enemyPrefabs.Length != 0)
        {
            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                GameObject enemyPrefab = enemyPrefabs[i];
                for (int j = 0; j < enemyPrefabsSpawnCount[i]; j++)
                {
                    bool placed = false;
                    do
                    {
                        int x = StateMigrator.random.Next(0, Width);
                        int z = StateMigrator.random.Next(0, Depth);
                        if (mapData[x, z] == TerrainType.Floor_01)
                        {
                            GameObject spawnedPrefab = Instantiate(enemyPrefab);
                            spawnedPrefab.transform.position = new Vector3(m_minX + (x * NodeSize), 0.0f, m_minZ + (z * NodeSize));
                            placed = true;
                        }
                    }
                    while (!placed);
                }
            }
        }
    }
}
