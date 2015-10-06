using UnityEngine;

/// <summary>
/// Represents a weighted graph edge.
/// </summary>
public class WeightedEdge
{
    public Vector3 Source { get { return this.m_source; } }
    public Vector3 Target { get { return this.m_target; } }
    public double Weight { get { return this.m_weight; } }

    private Vector3 m_source;
    private Vector3 m_target;
    private double m_weight;

    public WeightedEdge(Vector3 source, Vector3 target, double weight)
    {
        m_source = source;
        m_target = target;
        m_weight = weight;
    }
}