using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator
{
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

    public int[,] MapData { get { return m_mapData; } }
    public int[,] ObjectData { get { return m_objectData; } }
    public int[,] ItemData { get { return m_itemData; } }

    private int m_seed = 0;

    private int m_width = 0;
    private int m_depth = 0;
    private int m_middleX = 0;
    private int m_middleY = 0;

    private int m_spawnMinX;
    private int m_spawnMaxX;
    private int m_spawnMinY;
    private int m_spawnMaxY;

    private List<Room> m_rooms;

    private int[,] m_mapData;
    private int[,] m_objectData;
    private int[,] m_itemData;
    


    public MapGenerator(int width, int depth, int seed)
    {
        Reset(width, depth, seed);
    }

    void Reset(int width, int depth, int seed)
    {
        Random.seed = seed;
        m_seed = seed;
        m_width = width;
        m_depth = depth;
        m_middleX = m_width / 2;
        m_middleY = m_depth / 2;
        m_rooms = new List<Room>();
        m_mapData = MapHelpers.EmptyMap(m_width, m_depth, -1);
        m_objectData = MapHelpers.EmptyMap(m_width, m_depth, -1);
    }


    public void Generate()
    {
        int[,] data = null;
        GenerateChasmsAndLakes(ref data);
        int[,] mapData = MapHelpers.EmptyMap(m_width, m_depth,
            TerrainType.Impassable_Rubble_01);
        for (int x = 0; x < m_width; x++)
        {
            for (int z = 0; z < m_depth; z++)
            {
                if (data[x, z] == 1)
                {
                    mapData[x, z] = TerrainType.Chasm;
                }
                if (data[x, z] == 2)
                {
                    mapData[x, z] = TerrainType.Water_Shallow;
                }
            }
        }
        GenerateRooms();
        SeparateRooms();
        WriteRooms(ref mapData);
        FindMinSpanTree();
        GenerateCorridors(ref mapData);
        MapHelpers.Smooth(ref data, SmoothingThreshold, SmoothingIterations);



        GenerateWalls(ref mapData);
        //GenerateStairwells(ref mapData);
        m_mapData = mapData;

        m_objectData = MapHelpers.EmptyMap(m_width, m_depth, -1);
        GenerateTorches(ref m_objectData);
        GenerateChests(ref m_objectData);
    }


    private void GenerateChasmsAndLakes(ref int[,] data)
    {
        CellularAutomaton automaton = new CellularAutomaton(96, 96);
        automaton.Spawn(0.55f);
        automaton.Iterate(6, 4, 10);
        data = automaton.Data; 
        MapHelpers.Smooth(ref data, 4, 3);
        for (int i = 0; i <= 2; i++)
        {
            Point point = new Point(
                UnityEngine.Random.Range(0, m_width - 1),
                UnityEngine.Random.Range(0, m_depth - 1));
            int sample = data[point.X, point.Y];
            if (sample == TerrainType.Generator_Default)
            {
                MapHelpers.FloodFill(ref data, point, 
                    TerrainType.Generator_Default, 
                    TerrainType.Generator_Secondary);  
            }
        }
    }


 

    Point SpawnUpStairwell(ref int[,] data)
    {
        for (int x = 0; x < m_width; x++)
        {
            for (int z = 0; z < m_depth; z++)
            {
                if (m_mapData[x, z] == TerrainType.Wall_Stone_01)
                {
                    bool isHorizontalSection = false;
                    bool isVerticalSection = false;
                    if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 1, x + 1, z, z), TerrainType.Wall_Stone_01))
                    {
                        isHorizontalSection = true;
                    }
                    if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x, x, z - 1, z + 1),
                        TerrainType.Wall_Stone_01))
                    {
                        isVerticalSection = true;
                    }
                    if (isHorizontalSection && isVerticalSection)
                        continue;
                    float stairChance = 0.035f;
                    if (isHorizontalSection)
                    {
                        if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 1, x + 1, z + 1, z + 3),
                            TerrainType.Impassable_Rubble_01))
                        {
                            if (Random.value < stairChance)
                            {
                                data[x, z] = TerrainType.Stairs_UpNorth;
                                return new Point(x, z);
                            }
                        }
                        else if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 1, x + 1, z - 1, z - 3),
                            TerrainType.Impassable_Rubble_01))
                        {
                            if (Random.value < stairChance)
                            {
                                data[x, z] = TerrainType.Stairs_UpSouth;
                                return new Point(x, z);
                            }
                        }
                    }
                    if (isVerticalSection)
                    {
                        if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x + 1, x + 3, z - 1, z + 1),
                            TerrainType.Impassable_Rubble_01))
                        {
                            if (Random.value < stairChance)
                            {
                                data[x, z] = TerrainType.Stairs_UpEast;
                                return new Point(x, z);
                            }
                        }
                        else if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 3, x - 1, z - 1, z + 1),
                            TerrainType.Impassable_Rubble_01))
                        {
                            if (Random.value < stairChance)
                            {
                                data[x, z] = TerrainType.Stairs_UpWest;
                                return new Point(x, z);
                            }
                        }
                    }
                }
            }
        }
        return new Point(-1, -1);
    }

    Point SpawnDownStairwell(ref int[,] data, Point firstPos)
    {
        for (int x = 0; x < m_width; x++)
        {
            for (int z = 0; z < m_depth; z++)
            {
                if (m_mapData[x, z] == TerrainType.Wall_Stone_01)
                {
                    bool isHorizontalSection = false;
                    bool isVerticalSection = false;
                    if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 1, x + 1, z, z),
                        TerrainType.Wall_Stone_01))
                    {
                        isHorizontalSection = true;
                    }
                    if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x, x, z - 1, z + 1),
                        TerrainType.Wall_Stone_01))
                    {
                        isVerticalSection = true;
                    }
                    if (isHorizontalSection && isVerticalSection)
                        continue;
                    float stairChance = 0.15f;
                    if (isHorizontalSection)
                    {
                        if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 1, x + 1, z + 1, z + 3),
                            TerrainType.Impassable_Rubble_01))
                        {
                            if (Random.value < stairChance && Vector2.Distance(new Vector2(x,z), new Vector2(firstPos.X, firstPos.Y)) > 45)
                            {
                                data[x, z] = TerrainType.Stairs_UpNorth;
                                return new Point(x, z);
                            }
                        }
                        else if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 1, x + 1, z - 1, z - 3),
                            TerrainType.Impassable_Rubble_01))
                        {
                            if (Random.value < stairChance && Vector2.Distance(new Vector2(x, z), new Vector2(firstPos.X, firstPos.Y)) > 45)
                            {
                                data[x, z] = TerrainType.Stairs_UpSouth;
                                return new Point(x, z);
                            }
                        }
                    }
                    if (isVerticalSection)
                    {
                        if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x + 1, x + 3, z - 1, z + 1),
                            TerrainType.Impassable_Rubble_01))
                        {
                            if (Random.value < stairChance && Vector2.Distance(new Vector2(x, z), new Vector2(firstPos.X, firstPos.Y)) > 45)
                            {
                                data[x, z] = TerrainType.Stairs_UpEast;
                                return new Point(x, z);
                            }
                        }
                        else if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 3, x - 1, z - 1, z + 1),
                            TerrainType.Impassable_Rubble_01))
                        {
                            if (Random.value < stairChance && Vector2.Distance(new Vector2(x, z), new Vector2(firstPos.X, firstPos.Y)) > 45)
                            {
                                data[x, z] = TerrainType.Stairs_UpWest;
                                return new Point(x, z);
                            }
                        }
                    }
                }
            }
        }
        return new Point(-1, -1);
    }

    void GenerateStairwells(ref int[,] data)
    {
        Point point = new Point(-1, -1);
        while (point.X < 0 || point.Y < 0)
        {
            point = SpawnUpStairwell(ref data);
        }
        Point point2 = new Point(-1, -1);
        while (point2.X < 0 || point2.Y < 0)
        {
            point2 = SpawnDownStairwell(ref data, point);
        }
    }

    void GenerateChests(ref int[,] data)
    {
        for (int x = 0; x < m_width; x++)
        {
            for (int z = 0; z < m_depth; z++)
            {
                if (m_mapData[x, z] == (int)TerrainType.Wall_Stone_01)
                {
                    int neighbours = MapHelpers.CountNeighborsOfType(
                        ref m_mapData, x, z, TerrainType.Floor_01);
                    if (neighbours == 3)
                    {
                        float chestChance = 0.035f;
                        if (Random.value < chestChance)
                        {
                            // Scan region to the north.
                            if (z + 3 < m_depth && x - 1 > 0 && x + 1 < m_width)
                            {
                                if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 1, z + 1, x + 1, z + 3),
                                    TerrainType.Floor_01))
                                {
                                    data[x, z] = TerrainType.Item_Chest_North;
                                    continue;
                                }
                            }
                            // Scan region to the south.
                            if (z - 3 > 0 && x - 1 > 0 && x + 1 < m_width)
                            {
                                if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 1, z - 3, x + 1, z - 1),
                                    TerrainType.Floor_01))
                                {
                                    data[x, z] = TerrainType.Item_Chest_South;
                                    continue;
                                }
                            }
                            // Scan region to the south
                            if (x - 3 > 0 && z + 1 < m_width && z - 1 > 0)
                            {
                                if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 3, z - 1, x - 1, z + 1),
                                    TerrainType.Floor_01))
                                {
                                    data[x, z] = TerrainType.Item_Chest_East;
                                    continue;
                                }
                            }

                            // Scan region to the south             
                            if (x + 3 < m_width && z + 1 < m_width && z - 1 > 0)
                            {
                                if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x + 1, z - 1, x + 3, z + 1),
                                    TerrainType.Floor_01))
                                {
                                    data[x, z] = TerrainType.Item_Chest_West;
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void GenerateTorches(ref int[,] data)
    {
        for (int x = 0; x < m_width; x++)
        {
            for (int z = 0; z < m_depth; z++)
            {
                if (m_mapData[x, z] == (int)TerrainType.Wall_Stone_01)
                {
                    int neighbours = MapHelpers.CountNeighborsOfType(
                        ref m_mapData, x, z, TerrainType.Floor_01);
                    if (neighbours == 3)
                    {
                        float torchChance = 0.15f;
                        if (Random.value < torchChance)
                        {             
                            //// Scan region to the north.
                            if (z + 3 < m_depth && x - 1 > 0 && x + 1 < m_width)
                            {
                                if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 1, z + 1, x + 1, z + 3),
                                    TerrainType.Floor_01))
                                {
                                    data[x, z] = TerrainType.Torch_WallMounted_North;
                                    continue;
                                }
                            }
                            // Scan region to the south.
                            if (z - 3 > 0 && x - 1 > 0 && x + 1 < m_width)
                            {
                                if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 1, z-3, x+1, z-1),
                                    TerrainType.Floor_01))
                                {
                                    data[x, z] = TerrainType.Torch_WallMounted_South;
                                    continue;
                                }
                            }
                            // Scan region to the west
                            if (x - 3 > 0 && z + 1 < m_width && z - 1 > 0)
                            {
                                if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x - 3, z-1, x - 1, z + 1),
                                    TerrainType.Floor_01))
                                {
                                    data[x, z] = TerrainType.Torch_WallMounted_West;
                                    continue;
                                }
                            }

                            // Scan region to the east             
                            if (x + 3 < m_width && z + 1 < m_width && z - 1 > 0)
                            {
                                if (MapHelpers.IsUniformRegion(m_mapData, new Rectangle(x + 1, z - 1, x + 3, z + 1),
                                    TerrainType.Floor_01))
                                {
                                    data[x, z] = TerrainType.Torch_WallMounted_East;
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
    }


 

    void GenerateCorridors(ref int[,] data)
    {
        foreach (WeightedEdge edge in m_mst)
        {
            float x = edge.Source.x;
            float y = edge.Source.y;
            Vector2 position = new Vector2(edge.Source.x, edge.Source.y);
            Vector2 end = new Vector2(edge.Target.x, edge.Target.y);
            Vector2 direction = (end - position).normalized;
            float delta = 0.001f;
            float threshold = 0.1f;
            while (Vector2.Distance(position, end) > threshold)
            {
                position += direction * delta;
                int ix = (int)Mathf.Round(position.x);
                int iy = (int)Mathf.Round(position.y);
                data[ix, iy] = (int)TerrainType.Floor_01;
                data[ix, iy + 1] = (int)TerrainType.Floor_01;
            }
        }
    }

    // 
    void WriteRooms(ref int[,] data)
    {
        foreach (Room room in m_rooms)
        {
            MapHelpers.FillRectangle(ref data, room.Rectangle, TerrainType.Floor_01);
        }
    }




    List<WeightedEdge> m_mst;
    void FindMinSpanTree()
    {
        List<Vector3> points = new List<Vector3>();
        foreach (Room room in m_rooms)
        {
            points.Add(room.Rectangle.center);
        }

        m_mst = MapHelpers.FindMinimumSpanningTree(points, WeightedGraphEdgeRadius, points[0]);


//        GameObject.Find("Main Camera").GetComponent<DrawMST>().MST = m_mst;
    }

 

    private void GenerateRooms()
    {
        m_spawnMinX = m_middleX - SpawnDistance;
        m_spawnMaxX = m_middleX + SpawnDistance;
        m_spawnMinY = m_middleY - SpawnDistance;
        m_spawnMaxY = m_middleY + SpawnDistance;
        for (int i = 0; i < RoomCount; i++)
        {
            // Generate widths and heights until we have a pair with 
            // an acceptable width to height ratio. Widths and heights
            // are sampled from an exponential distribution in order 
            // to favour generation of smaller cells. 
            int w = 0, h = 0;
            bool isRatioAcceptable = false;
            while (!isRatioAcceptable)
            {
                w = (int)Mathf.Round((float)SampleDistribution(1, 1) *
                    (MaxRoomWidth - MinRoomWidth) + MinRoomWidth);
                h = (int)Mathf.Round((float)SampleDistribution(1, 1) *
                    (MaxRoomHeight - MinRoomHeight) + MinRoomHeight);

                isRatioAcceptable = (w >= h) ?
                    ((float)h / w) > MinRoomSideRatio :
                    ((float)w / h) > MinRoomSideRatio;
            }
            // Calculate random spawn position.
            int x = (int)Mathf.Round(Random.value *
                (m_spawnMaxX - m_spawnMinX) + m_spawnMinX);
            int y = (int)Mathf.Round(Random.value *
                (m_spawnMaxY - m_spawnMinY) + m_spawnMinY);

            m_rooms.Add(new Room(x, y, w, h));
        }
    }

    void SeparateRooms()
    {
        int separationIterations = 0;
        bool separationComplete = false;
        while (!separationComplete)
        {
            separationComplete = true;
            foreach (Room room in m_rooms)
            {
                if (!room.Stopped)
                {
                    Vector2 force = CalculateSeparation(m_rooms, room);
                    if (force.magnitude > 0.5f)
                    {
                        separationComplete = false;
                        room.Velocity.x += force.x * SeparationStrength;
                        room.Velocity.y += force.y * SeparationStrength;
                        room.Velocity.Normalize();
                        room.Velocity *= 5f;

                        room.Rectangle.x += room.Velocity.x;
                        room.Rectangle.y += room.Velocity.y;

                        room.Rectangle.x = Mathf.Round(room.Rectangle.x);
                        room.Rectangle.y = Mathf.Round(room.Rectangle.y);


                        if (room.Rectangle.x < 2)
                        {
                            room.Rectangle.x = 2;
                        }
                        if (room.Rectangle.xMax > m_width - 3)
                        {
                            room.Rectangle.x = m_width - (room.Rectangle.width + 3);
                        }
                        if (room.Rectangle.y < 2)
                        {
                            room.Rectangle.y = 2;
                        }
                        if (room.Rectangle.yMax > m_depth - 3)
                        {
                            room.Rectangle.y = m_depth - (room.Rectangle.height + 3);
                        }
                    }
                    else room.Stopped = true;
                }
            }
            separationIterations++;
            if (separationIterations > 25000) break;
        }
    }



   

    private void GenerateWalls(ref int[,] data)
    {
        int width = data.GetLength(0);
        int height = data.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (data[x, z] == (int)TerrainType.Impassable_Rubble_01)
                {
                    bool convertToWall = false;
                    if (x != 0)
                        if (data[x - 1, z] == (int)TerrainType.Floor_01)
                            convertToWall = true;
                    if (x != m_width - 1)
                        if (data[x + 1, z] == (int)TerrainType.Floor_01)
                            convertToWall = true;
                    if (z != 0)
                        if (data[x, z - 1] == (int)TerrainType.Floor_01)
                            convertToWall = true;
                    if (z != m_depth - 1)
                        if (data[x, z + 1] == (int)TerrainType.Floor_01)
                            convertToWall = true;
                    if (x != 0 && z != 0)
                        if (data[x - 1, z - 1] == (int)TerrainType.Floor_01)
                            convertToWall = true;
                    if (x != 0 && z != m_depth - 1)
                        if (data[x - 1, z + 1] == (int)TerrainType.Floor_01)
                            convertToWall = true;
                    if (x != m_width - 1 && z != 0)
                        if (data[x + 1, z - 1] == (int)TerrainType.Floor_01)
                            convertToWall = true;
                    if (x != m_width - 1 && z != m_depth - 1)
                        if (data[x + 1, z + 1] == (int)TerrainType.Floor_01)
                            convertToWall = true;
                    if (convertToWall)
                    {
                        
                        int c = 0;
                        if (x != 0)
                            if (data[x - 1, z] == (int)TerrainType.Chasm)
                                c++;
                        if (x != m_width - 1)
                            if (data[x + 1, z] == (int)TerrainType.Chasm)
                                c++;
                        if (z != 0)
                            if (data[x, z - 1] == (int)TerrainType.Chasm)
                                c++;
                        if (z != m_depth - 1)
                            if (data[x, z + 1] == (int)TerrainType.Chasm)
                                c++;
                        if (c <= 1)
                        {
                            data[x, z] = (int)TerrainType.Wall_Stone_01;
                        }
                    }
                }
            }
        }
    }






    double SampleDistribution(double factor, double upperLimit)
    {
        double result = double.MaxValue;
        while (result >= upperLimit)
        {
            result = -Mathf.Log(Random.value) / factor;
        }
        return result;
    }

    /// <summary>
    /// Calculates the total repulsive force from neighbouring cells.
    /// </summary>
    /// <param name="cells"></param>
    /// <param name="cell"></param>
    /// <returns></returns>   
    int cellSeparationNeighbourRadius = 25;
    private Vector2 CalculateSeparation(List<Room> cells, Room cell)
    {
        int neighbourCount = 0;
        int neighbourIntersections = 0;
        Vector2 separationVelocity = new Vector2();
        foreach (Room neighbour in cells)
        {
            if (cell != neighbour)
            {
                Vector2 cellCenter = new Vector2(
                    cell.Rectangle.center.x,
                    cell.Rectangle.center.y);
                Vector2 neighbourCenter = new Vector2(
                    neighbour.Rectangle.center.x,
                    neighbour.Rectangle.center.y);
                if (Vector2.Distance(cellCenter, neighbourCenter) <
                    cellSeparationNeighbourRadius)
                {
                    separationVelocity.x +=
                        neighbourCenter.x - cellCenter.x;
                    separationVelocity.y +=
                        neighbourCenter.y - cellCenter.y;
                    if (cell.Rectangle.Overlaps(neighbour.Rectangle))
                        neighbourIntersections++;
                    neighbourCount++;
                }
            }
        }
        if (neighbourIntersections > 0)
        {
            separationVelocity.x /= neighbourCount;
            separationVelocity.y /= neighbourCount;
            separationVelocity.x *= -1;
            separationVelocity.y *= -1;
            separationVelocity.Normalize();
            return separationVelocity;
        }
        return Vector2.zero;
    } 
}

