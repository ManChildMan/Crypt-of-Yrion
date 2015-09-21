using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script uses open GL to draw lines showing the edges of a minimum 
/// spanning tree.
/// </summary>
public class DrawMST : MonoBehaviour
{
    public int Width = 128;
    public int Depth = 128;
    public float NodeSize = 1;
    public Material Material;
    public List<WeightedEdge> MinimumSpanningTree = 
        new List<WeightedEdge>();

    void Draw()
    {
        foreach (WeightedEdge edge in MinimumSpanningTree)
        {
            Vector3 start = (edge.Source * NodeSize);
            start.x -= ((Width * NodeSize) / 2);
            start.y -= ((Depth * NodeSize) / 2);
            Vector3 end = (edge.Target * NodeSize);
            end.x -= ((Width * NodeSize) / 2);
            end.y -= ((Depth * NodeSize) / 2);
            GL.Begin(GL.LINES);
            Material.SetPass(0);
            GL.Color(new Color(Material.color.r, Material.color.g, Material.color.b, Material.color.a));
            GL.Vertex3(start.x, 1, start.y);
            GL.Vertex3(end.x, 1, end.y);
            GL.End();
        }
    }

    void OnPostRender()
    {
        Draw();
    }
}