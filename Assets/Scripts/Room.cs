using UnityEngine;

/// <summary>
/// This class is used by the map generator to represent rooms in map space
/// during procedural generation.
/// </summary>
public class Room
{
    public Rect Rectangle;
    public Vector2 Velocity;
    public bool Stopped = false;
    public Room(int x, int y, int width, int height)
    {
        Rectangle = new Rect(x, y, width, height);
        Velocity.x = Random.value * 10 - 5;
        Velocity.y = Random.value * 10 - 5;
    }
}

