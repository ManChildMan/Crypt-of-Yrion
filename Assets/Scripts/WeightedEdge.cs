
/// <summary>
/// Represents a weighted graph edge.
/// </summary>
using UnityEngine;
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