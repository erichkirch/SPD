using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room 
{
    public Rect rect;
    public Vector2 room_center;

    public Room(int x, int y, int width, int height)
    {
        rect = new Rect(x, y, width, height);
        room_center = new Vector2(Mathf.Round(x + width / 2), Mathf.Round(y + height / 2));
    }
}
