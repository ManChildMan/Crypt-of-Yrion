using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Put this script on a Camera
public class DrawMST : MonoBehaviour
{

    // Fill/drag these in from the editor

    // Choose the Unlit/Color shader in the Material Settings
    // You can change that color, to change the color of the connecting lines
    public Material lineMat;
    public float NodeSize = 1;
    public int Width = 128;
    public int Depth = 128;
    public List<WeightedEdge> MST = new List<WeightedEdge>();

    // Connect all of the `points` to the `mainPoint`
    void RenderMST()
    {

        foreach (WeightedEdge t in MST)
            {
                Vector3 start = (t.Source * NodeSize);
                start.x -= ((Width * NodeSize) / 2);
                start.y -= ((Depth * NodeSize) / 2);
                Vector3 end = (t.Target * NodeSize);
                end.x -= ((Width * NodeSize) / 2);
                end.y -= ((Depth * NodeSize) / 2);
                GL.Begin(GL.LINES);
                lineMat.SetPass(0);
                GL.Color(new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a));
                GL.Vertex3(start.x, 1, start.y);
                GL.Vertex3(end.x, 1, end.y);
                GL.End();
            }

    }

    void OnPostRender()
    {
        RenderMST();
    }

}