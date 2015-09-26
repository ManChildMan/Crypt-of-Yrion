
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using WeightedGraph = System.Collections.Generic.Dictionary<UnityEngine.Vector3,
    System.Collections.Generic.List<WeightedEdge>>;

public static class TerrainKey
{
    public const int Impassable = -100;
    public const int Impassable_Rubble_01 = -101;

    public const int Chasm = 0;

    public const int Floor = 100;
    public const int Floor_01 = 101;

    public const int Wall = 200;
    public const int Wall_Stone_01 = 201;

    public const int Water = 300;
    public const int Water_Shallow = 301;
    public const int Water_Deep = 302;

    public const int Torch = 400;
    public const int Torch_WallMounted_North = 401;
    public const int Torch_WallMounted_South = 402;
    public const int Torch_WallMounted_East = 403;
    public const int Torch_WallMounted_West = 404;

    public const int Item = 500;
    public const int Item_Chest_North = 501;
    public const int Item_Chest_South = 502;
    public const int Item_Chest_East = 503;
    public const int Item_Chest_West = 504;

    public const int Stairs = 600;
    public const int Stairs_UpNorth = 601;
    public const int Stairs_UpSouth = 602;
    public const int Stairs_UpEast = 603;
    public const int Stairs_UpWest = 604;
    public const int Stairs_DownNorth = 601;
    public const int Stairs_DownSouth = 602;
    public const int Stairs_DownEast = 603;
    public const int Stairs_DownWest = 604;

    public const int Generator_Empty = -1;
    public const int Generator_Default = 1;
    public const int Generator_Secondary = 2;
    public const int Generator_Selection = int.MaxValue;
}

public struct Point
{
    public int X, Y;
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
/// <summary>
/// 
/// </summary>
public class MapHelpers
{
    public const int GEN_EMPTY = 0;
    public const int GEN_DATA = 1;
    public const int GEN_HIGHLIGHT = int.MaxValue;

    public const int TILE_RUBBLE = -1;
    public const int TILE_EMPTY = 0;
    public const int TILE_STONEBLOCK = 1;

    /// <summary>
    /// This method converts a point cloud into an graph of undirected weighted 
    /// edges. Undirected graphs are used to calculate minimum spanning trees.
    /// In this implementation edges are weighted by distance and the radius of
    /// considered edge connections can be adjusted.
    /// </summary>
    public static WeightedGraph CreateWeightedGraphFromPoints(
        List<Vector3> points, float radius)
    {

        int nodeCount = points.Count;
        int connectionCount = 0;
        WeightedGraph connections = new WeightedGraph();

        for (int subjectIndex = 0; subjectIndex < points.Count;
            subjectIndex++)
        {
            Vector3 subjectPosition = points[subjectIndex];
            for (int neighbourIndex = 0; neighbourIndex < points.Count;
                neighbourIndex++)
            {
                if (subjectIndex != neighbourIndex)
                {
                    Vector3 neighbourPosition = points[neighbourIndex];
                    double distance = Vector3.Distance(
                        subjectPosition, neighbourPosition);
                    if (distance < radius)
                    {
                        if (!connections.ContainsKey(subjectPosition))
                            connections.Add(subjectPosition,
                                new List<WeightedEdge>());
                        connections[subjectPosition].Add(
                            new WeightedEdge(subjectPosition,
                                neighbourPosition, distance));
                        connectionCount += 2;
                    }
                }
            }
        }
        return connections;
    }

    /// <summary>
    /// 
    /// </summary>
    public static List<WeightedEdge> FindMinimumSpanningTree(
        List<Vector3> points, float radius, Vector3 firstPoint)
    {
        WeightedGraph graph = CreateWeightedGraphFromPoints(points, radius);
        return FindMinimumSpanningTree(graph, firstPoint);
    }

    /// <summary>
    /// 
    /// </summary>
    public static List<WeightedEdge> FindMinimumSpanningTree(
        WeightedGraph graph, Vector3 firstPoint)
    {
        List<WeightedEdge> minimumSpanningTree = new List<WeightedEdge>();
        List<Vector3> processedPoints = new List<Vector3>();
        processedPoints.Add(firstPoint);
        while (processedPoints.Count < graph.Count)     
        {
            // Determine all edges connected to processed vertices that 
            // don't target points already existing in the tree. Add the
            // edge with the smallest weight value to the tree.
            List<WeightedEdge> candidateEdges = new List<WeightedEdge>();
            foreach (Vector3 vertex in processedPoints)
            {
                foreach (WeightedEdge edge in graph[vertex])
                {
                    bool edgeTargetInTree = false;
                    foreach (Vector3 processedVertex in processedPoints)
                    {
                        if (edge.Target == processedVertex)
                            edgeTargetInTree = true;
                    }
                    if (!edgeTargetInTree)
                        candidateEdges.Add(edge);
                }
            }
            WeightedEdge nextEdge = candidateEdges[0];
            foreach (WeightedEdge edge in candidateEdges)
            {
                if (nextEdge.Weight > edge.Weight)
                    nextEdge = edge;
            }
            minimumSpanningTree.Add(nextEdge);

            // Keep track of processed points.
            processedPoints.Add(nextEdge.Target);
        }
        return minimumSpanningTree;
    }


   

    class AreaSample
    {
        public Point Position { get; set; }
        public int Area { get; set; }
    }

    public static int[,] ExtractLargestRegion(int[,] data, float sampleFraction = 0.45f, bool crop = true)
    {
        int width = data.GetUpperBound(0) + 1;
        int height = data.GetUpperBound(1) + 1;
        int[,] copy = new int[width, height];
        Array.Copy(data, copy, width * height);

        // Cache the area and position of a random sampling of rooms.
        int sampleCount = (int)Math.Round(
            width * height * Mathf.Clamp(sampleFraction, 0, 1));
        AreaSample[] samples = new AreaSample[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            AreaSample sample = new AreaSample();
            sample.Position = new Point(
                UnityEngine.Random.Range(0, width),
                UnityEngine.Random.Range(0, height));
            sample.Area = GetArea(copy, sample.Position, 1);
            samples[i] = sample;
        }

        // Find the largest area amongst the sampled areas.
        Point largestAreaPosition = new Point(-1, -1);
        int largestArea = -1;
        for (int i = 0; i < sampleCount; i++)
        {
            AreaSample sample = samples[i];
            if (sample.Area > largestArea)
            {
                largestAreaPosition = sample.Position;
                largestArea = sample.Area;
            }
        }
        // Flood fill largest area with int.MaxValue to make it distinct.    
        FloodFill(ref copy, largestAreaPosition, 1, int.MaxValue);
        // Get a bounding rectangle containing the largest region.
        Rectangle bounds = MapHelpers.GetBoundingRectangle(copy, int.MaxValue);




        if (crop)
        {
            int[,] room = EmptyMap(bounds.Right - bounds.X, bounds.Bottom - bounds.Y, 0); 
            for (int x = bounds.X; x < bounds.Right; x++)
            {
                for (int y = bounds.Y; y < bounds.Bottom; y++)
                {
                    room[x - bounds.X, y - bounds.Y] =
                        (copy[x, y] == int.MaxValue) ? 1 : 0;

                }
            }
            return room;
        }
        else
        {
            int[,] room = EmptyMap(width, height, 0);
            for (int x = bounds.X; x <= bounds.Right; x++)
            {
                for (int y = bounds.Y; y <= bounds.Bottom; y++)
                {
                    room[x, y] =
                        (copy[x, y] == int.MaxValue) ? 1 : 0;

                }
            }
            return room;
        }
    }
    // Fills the specified region with the supplied value.
    public static void FillRectangle(ref int[,] data, Rect rect, int value)
    {
        if (rect.x < 0 || rect.xMax > data.GetLength(0) - 1 ||
            rect.y < 0 || rect.yMax > data.GetLength(1) - 1)
            throw new ArgumentOutOfRangeException(
                "The specified region would exceed the bounds of the data array.");
        for (int x = (int)rect.x; x <= (int)rect.xMax; x++)
        {
            for (int z = (int)rect.y; z <= (int)rect.yMax; z++)
            {
                data[x, z] = value;
            }
        }
    }
    // Returns true if all map elements in the specified region have the same
    // value.
    public static bool IsUniformRegion(int[,] data, Rectangle rect, int value)
    {
        if (rect.X < 0 || rect.Right > data.GetLength(0) - 1 || 
            rect.Y < 0 || rect.Bottom > data.GetLength(1) - 1)
            throw new ArgumentOutOfRangeException(
                "The specified region would exceed the bounds of the data array.");
        for (int x = rect.X; x <= rect.Right; x++)
        {
            for (int z = rect.Y; z <= rect.Bottom; z++)
            {
                if (data[x, z] != value)
                {
                    return false;
                }
            }
        }
        return true;
    }




    public static Rectangle GetBoundingRectangle(int[,] data, int target)
    {
        int width = data.GetUpperBound(0) + 1;
        int height = data.GetUpperBound(1) + 1;
        Point min = new Point(width - 1, height - 1);
        Point max = new Point(0, 0);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (data[x, y] == target)
                {
                    if (x < min.X) min.X = x;
                    if (x > max.X) max.X = x;
                    if (y < min.Y) min.Y = y;
                    if (y > max.Y) max.Y = y;
                }
            }
        }
        return new Rectangle(min.X, min.Y, max.X, max.Y);
    }



    public static int GetArea(int[,] data, Point start, int target)
    {
        int width = data.GetUpperBound(0) + 1;
        int height = data.GetUpperBound(1) + 1;
        int[,] copy = new int[width, height];
        Array.Copy(data, copy, width * height);
        return FloodFill(ref copy, start, target, -2);
    }



    public static int FloodFill(ref int[,] data, Point start, int target, int replacement)
    {
        int filledArea = 0;
        if (data[start.X, start.Y] != target)
            return filledArea;
        int width = data.GetUpperBound(0) + 1;
        int height = data.GetUpperBound(1) + 1;
        Queue<Point> nodes = new Queue<Point>();
        nodes.Enqueue(start);
        while (nodes.Count > 0)
        {
            Point node = nodes.Dequeue();
            if (data[node.X, node.Y] == target)
            {
                data[node.X, node.Y] = replacement;
                filledArea++;
            }
            if (node.X > 0 && data[node.X - 1, node.Y] == target)
            {
                nodes.Enqueue(new Point(node.X - 1, node.Y));
                data[node.X - 1, node.Y] = replacement;
                filledArea++;
            }
            if (node.X < width - 1 && data[node.X + 1, node.Y] == target)
            {
                nodes.Enqueue(new Point(node.X + 1, node.Y));
                data[node.X + 1, node.Y] = replacement;
                filledArea++;
            }
            if (node.Y > 0 && data[node.X, node.Y - 1] == target)
            {
                nodes.Enqueue(new Point(node.X, node.Y - 1));
                data[node.X, node.Y - 1] = replacement;
                filledArea++;
            }
            if (node.Y < height - 1 && data[node.X, node.Y + 1] == target)
            {
                nodes.Enqueue(new Point(node.X, node.Y + 1));
                data[node.X, node.Y + 1] = replacement;
                filledArea++;
            }
        }
        return filledArea;
    }

    public static int[,] EmptyMap(int width, int height, int value)
    {
        int[,] data = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                data[x, y] = value;
            }
        }
        return data;
    }

    public static int CountNeighborsOfType(ref int[,] data, int x, int z, int type)
    {
        int width = data.GetLength(0);
        int depth = data.GetLength(1);
        int count = 0;
        // Check cell on the right.
        if (x != width - 1)
            if (data[x + 1, z] == type)
                count++;
        // Check cell on the bottom right.
        if (x != width - 1 && z != depth - 1)
            if (data[x + 1, z + 1] == type)
                count++;
        // Check cell on the bottom.
        if (z != depth - 1)
            if (data[x, z + 1] == type)
                count++;
        // Check cell on the bottom left.
        if (x != 0 && z != depth - 1)
            if (data[x - 1, z + 1] == type)
                count++;
        // Check cell on the left.
        if (x != 0)
            if (data[x - 1, z] == type)
                count++;
        // Check cell on the top left.
        if (x != 0 && z != 0)
            if (data[x - 1, z - 1] == type)
                count++;
        // Check cell on the top.
        if (z != 0)
            if (data[x, z - 1] == type)
                count++;
        // Check cell on the top right.
        if (x != width - 1 && z != 0)
            if (data[x + 1, z - 1] == type)
                count++;
        return count;
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
                            if (data[x + 1, y] == 1)
                                count++;
                        // Check cell on the bottom.
                        if (y != height - 1)
                            if (data[x, y + 1] == 1)
                                count++;
                        // Check cell on the left.
                        if (x != 0)
                            if (data[x - 1, y] == 1)
                                count++;
                        // Check cell on the top.
                        if (y != 0)
                            if (data[x, y - 1] == 1)
                                count++;
                        if (x != 0 && y != 0)
                            if (data[x - 1, y - 1] == 1)
                                count++;
                        if (x != 0 && y != height - 1)
                            if (data[x - 1, y + 1] == 1)
                                count++;
                        if (x != width - 1 && y != 0)
                            if (data[x + 1, y - 1] == 1)
                                count++;
                        if (x != width - 1 && y != height - 1)
                            if (data[x + 1, y + 1] == 1)
                                count++;
                        if (count > threshold) data[x, y] = 1;
                    }
                }
            }
        }
        return removed;
    }
}
    public struct Rectangle
    {
        public int X, Y, Right, Bottom;
        public Rectangle(int x, int y, int right, int bottom)
        {
            X = x;
            Y = y;
            Right = right;
            Bottom = bottom;
        }
        public static Rectangle ToRectangle(Rect rect)
        {
            return new Rectangle(
                (int)Mathf.Max(rect.xMin, rect.xMax), 
                (int)Mathf.Min(rect.xMin, rect.xMax),
                (int)Mathf.Max(rect.yMin, rect.yMax), 
                (int)Mathf.Min(rect.yMin, rect.yMax));
        }
    }


/// <summary>
/// Represents a weighted graph edge.
/// </summary>
public class WeightedEdge
{
    Vector3 m_source;
    Vector3 m_target;
    double m_weight;
    public Vector3 Source { get { return this.m_source; } }
    public Vector3 Target { get { return this.m_target; } }
    public double Weight { get { return this.m_weight; } }
    public WeightedEdge(Vector3 source, Vector3 target, double weight)
    {
        m_source = source;
        m_target = target;
        m_weight = weight;
    }
}

/// <summary>
/// 
/// </summary>
public class CellularAutomaton
{
    // Represents a single cell.
    class Cell
    {
        public Vector2 Position { get; private set; }
        public bool IsAlive { get; set; }

        public Cell(Vector2 position)
        {
            Position = position;
            IsAlive = false;
        }
    }

    public int[,] Data
    {
        get { return m_cells; }
    }

    private int m_width;
    private int m_depth;
    private int[,] m_cells;
    private const int DEAD = (int)TerrainKey.Generator_Empty;
    private const int ALIVE = (int)TerrainKey.Generator_Default;

    public CellularAutomaton(int width, int depth)
    {
        m_width = width;
        m_depth = depth;
        m_cells = MapHelpers.EmptyMap(m_width, m_depth,
            (int)TerrainKey.Generator_Empty);
    }

    public void MakeAlive(float fraction)
    {
        int quantity = (int)Mathf.Round(m_width * m_depth * 
            Mathf.Clamp(fraction, 0, 1));
        while (quantity > 0)
        {
            int x = UnityEngine.Random.Range(0, m_width - 1);
            int z = UnityEngine.Random.Range(0, m_depth - 1);
            if (m_cells[x, z] == DEAD)
            {
                m_cells[x, z] = ALIVE;
                quantity--;
            }
        }
    }

    public void Iterate(int birthThreshold, int survivalThreshold,
        int iterations)
    {
        int neighbours = -1;
        bool isAlive = false;
        for (int i = 0; i < iterations; i++)
        {
            for (int x = 0; x < m_width; x++)
            {
                for (int z = 0; z < m_depth; z++)
                {
                    neighbours = GetLivingNeighbors(x, z);

                    isAlive = m_cells[x, z] == ALIVE;
                    if (isAlive)
                    {
                        if (neighbours >= survivalThreshold)
                            isAlive = true;
                        else isAlive = false;
                    }
                    else if (neighbours >= birthThreshold)
                        isAlive = true;

                    m_cells[x, z] = isAlive ? ALIVE : DEAD;
                }
            }
        }
    }

    private int GetLivingNeighbors(int x, int z)
    {
        int count = 0;
        // Check cell on the right.
        if (x != m_width - 1)
            if (m_cells[x + 1, z] == ALIVE)
                count++;
        // Check cell on the bottom right.
        if (x != m_width - 1 && z != m_depth - 1)
            if (m_cells[x + 1, z + 1] == ALIVE)
                count++;
        // Check cell on the bottom.
        if (z != m_depth - 1)
            if (m_cells[x, z + 1] == ALIVE)
                count++;
        // Check cell on the bottom left.
        if (x != 0 && z != m_depth - 1)
            if (m_cells[x - 1, z + 1] == ALIVE)
                count++;
        // Check cell on the left.
        if (x != 0)
            if (m_cells[x - 1, z] == ALIVE)
                count++;
        // Check cell on the top left.
        if (x != 0 && z != 0)
            if (m_cells[x - 1, z - 1] == ALIVE)
                count++;
        // Check cell on the top.
        if (z != 0)
            if (m_cells[x, z - 1] == ALIVE)
                count++;
        // Check cell on the top right.
        if (x != m_width - 1 && z != 0)
            if (m_cells[x + 1, z - 1] == ALIVE)
                count++;
        return count;
    }

    public static int[,] GetCellularAutomata()
    {
        CellularAutomaton ca = new CellularAutomaton(96, 96);
        ca.MakeAlive(0.55f);
        ca.Iterate(6, 4, 2);

        int[,] data = ca.Data; // MapHelpers.ExtractLargestRegion(ca.Data, 0.5f, false);

        MapHelpers.Smooth(ref data, 4, 3);
        return data;
        
    }
}