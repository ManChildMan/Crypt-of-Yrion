
using UnityEngine;


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

