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