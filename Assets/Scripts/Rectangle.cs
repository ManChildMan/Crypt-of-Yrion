using UnityEngine;

/// <summary>
/// 
/// </summary>
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
