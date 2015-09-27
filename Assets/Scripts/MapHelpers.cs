
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using WeightedGraph = System.Collections.Generic.Dictionary<UnityEngine.Vector3,
    System.Collections.Generic.List<WeightedEdge>>;

/// <summary>
/// 
/// </summary>
public class MapHelpers
{
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
    //
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
        int width = data.GetLength(0) - 1;
        int depth = data.GetLength(1) - 1;
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
    /// <summary>
    /// Smooths generator-level map data.
    /// </summary>
    /// <param name="data">A 2D array of generator-level map data (-1s & 1s).</param>
    /// <param name="threshold">The number of neighbours required to trigger change.</param>
    /// <param name="iterations">The number of times to perform smoothing.</param>
    /// <returns></returns>
    public static int Smooth(ref int[,] data, int threshold, int iterations)
    {
        int width = data.GetLength(0) - 1;
        int height = data.GetLength(1) - 1;
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







//private class AreaSample
//{
//    public Point Position { get; set; }
//    public int Area { get; set; }
//}

//public static int[,] ExtractLargestRegion(int[,] data, float sampleFraction = 0.45f, bool crop = true)
//{
//    int width = data.GetUpperBound(0) + 1;
//    int height = data.GetUpperBound(1) + 1;
//    int[,] copy = new int[width, height];
//    Array.Copy(data, copy, width * height);

//    // Cache the area and position of a random sampling of rooms.
//    int sampleCount = (int)Math.Round(
//        width * height * Mathf.Clamp(sampleFraction, 0, 1));
//    AreaSample[] samples = new AreaSample[sampleCount];
//    for (int i = 0; i < sampleCount; i++)
//    {
//        AreaSample sample = new AreaSample();
//        sample.Position = new Point(
//            UnityEngine.Random.Range(0, width),
//            UnityEngine.Random.Range(0, height));
//        sample.Area = GetArea(copy, sample.Position, 1);
//        samples[i] = sample;
//    }

//    // Find the largest area amongst the sampled areas.
//    Point largestAreaPosition = new Point(-1, -1);
//    int largestArea = -1;
//    for (int i = 0; i < sampleCount; i++)
//    {
//        AreaSample sample = samples[i];
//        if (sample.Area > largestArea)
//        {
//            largestAreaPosition = sample.Position;
//            largestArea = sample.Area;
//        }
//    }

//    // Flood fill largest area with int.MaxValue to make it distinct.    
//    FloodFill(ref copy, largestAreaPosition, 1, int.MaxValue);

//    // Get a bounding rectangle containing the largest region.
//    Rectangle bounds = MapHelpers.GetBoundingRectangle(copy, int.MaxValue);

//    if (crop)
//    {
//        int[,] room = EmptyMap(bounds.Right - bounds.X, bounds.Bottom - bounds.Y, 0); 
//        for (int x = bounds.X; x < bounds.Right; x++)
//        {
//            for (int y = bounds.Y; y < bounds.Bottom; y++)
//            {
//                room[x - bounds.X, y - bounds.Y] = 
//                    (copy[x, y] == int.MaxValue) ? 1 : 0;
//            }
//        }
//        return room;
//    }
//    else
//    {
//        int[,] room = EmptyMap(width, height, 0);
//        for (int x = bounds.X; x <= bounds.Right; x++)
//        {
//            for (int y = bounds.Y; y <= bounds.Bottom; y++)
//            {
//                room[x, y] = (copy[x, y] == int.MaxValue) ? 1 : 0;

//            }
//        }
//        return room;
//    }
//}

