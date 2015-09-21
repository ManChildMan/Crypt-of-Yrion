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
    private int[,] m_data;

    public int[,] Data { get { return m_data; } }

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
        m_data = GetEmptyMap(m_width, m_depth);
    }



    public void Generate()
    {
        SpawnRooms();
        SeparateRooms();
        DrawRooms();

        FindMST();
        DrawCorridors();

        if (UseSmoothing)
            Smooth(ref m_data, SmoothingThreshold, SmoothingIterations);

        DrawWalls(ref m_data);
    }

    void DrawCorridors()
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
                m_data[ix, iy] = 0;
                m_data[ix, iy + 1] = 0;
                //m_data[ix - 1, iy] = 0;
                //m_data[ix - 1, iy + 1] = 0;
            }
        }
    }

    void DrawRooms()
    {
        foreach (Room room in m_rooms)
        {
            FillRectangle(ref m_data, room.Rectangle, 0);
        }
    }

    List<WeightedEdge> m_mst;
    void FindMST()
    {
        List<Vector3> points = new List<Vector3>();
        foreach (Room room in m_rooms)
        {
            points.Add(room.Rectangle.center);
        }

        m_mst = MapHelpers.FindMinimumSpanningTree(points, WeightedGraphEdgeRadius, points[0]);


//        GameObject.Find("Main Camera").GetComponent<DrawMST>().MST = m_mst;
    }

 

    void SpawnRooms()
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



   

    private void DrawWalls(ref int[,] data)
    {
        int width = data.GetLength(0);
        int height = data.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (data[x, z] == -1)
                {
                    bool convertToWall = false;
                    if (x != 0)
                        if (data[x - 1, z] == 0)
                            convertToWall = true;
                    if (x != m_width - 1)
                        if (data[x + 1, z] == 0)
                            convertToWall = true;
                    if (z != 0)
                        if (data[x, z - 1] == 0)
                            convertToWall = true;
                    if (z != m_depth - 1)
                        if (data[x, z + 1] == 0)
                            convertToWall = true;
                    if (x != 0 && z != 0)
                        if (data[x - 1, z - 1] == 0)
                            convertToWall = true;
                    if (x != 0 && z != m_depth - 1)
                        if (data[x - 1, z + 1] == 0)
                            convertToWall = true;
                    if (x != m_width - 1 && z != 0)
                        if (data[x + 1, z - 1] == 0)
                            convertToWall = true;
                    if (x != m_width - 1 && z != m_depth - 1)
                        if (data[x + 1, z + 1] == 0)
                            convertToWall = true;
                    if (convertToWall) data[x, z] = 1;
                }
            }
        }
    }


    private void FillRectangle(ref int[,] data, Rect rect, int value)
    {
        int xMin = (int)Mathf.Round(rect.x);
        int xMax = (int)Mathf.Round(rect.xMax);
        int yMin = (int)Mathf.Round(rect.y);
        int yMax = (int)Mathf.Round(rect.yMax);
        for (int x = xMin; x <= xMax; x++)
        {    
            for (int y = yMin; y <= yMax; y++)
            {
                data[x, y] = value;
            }
        }
    }




    public static int[,] GetEmptyMap(int width, int height)
    {
        int[,] data = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                data[x, y] = -1;
            }
        }
        return data;
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
        //if (neighbours == 0)
        //    return velocity;
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

    public static int Smooth(ref int[,] data, int threshold, int iterations)
    {
        int width = data.GetUpperBound(0) + 1;
        int height = data.GetUpperBound(1) + 1;
        int removed = 0;
        for (int i = 0; i < iterations; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (data[x, y] == -1)
                    {
                        int count = 0;
                        // Check cell on the right.
                        if (x != width - 1)
                            if (data[x + 1, y] == 0)
                                count++;
                        // Check cell on the bottom.
                        if (y != height - 1)
                            if (data[x, y + 1] == 0)
                                count++;
                        // Check cell on the left.
                        if (x != 0)
                            if (data[x - 1, y] == 0)
                                count++;
                        // Check cell on the top.
                        if (y != 0)
                            if (data[x, y - 1] == 0)
                                count++;
                        if (x != 0 && y != 0)
                            if (data[x - 1, y - 1] == 0)
                                count++;
                        if (x != 0 && y != height - 1)
                            if (data[x - 1, y + 1] == 0)
                                count++;
                        if (x != width - 1 && y != 0)
                            if (data[x + 1, y - 1] == 0)
                                count++;
                        if (x != width - 1 && y != height - 1)
                            if (data[x + 1, y + 1] == 0)
                                count++;
                        if (count > threshold) data[x, y] = 0;
                    }
                }
            }
        }
        return removed;
    }
}

