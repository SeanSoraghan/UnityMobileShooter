using UnityEngine;
using System.Collections;

public class SMath
{
    public static Vector2[] CreatePlaneFromPoint (Vector2 Point, float width, float height)
    {
        Vector2[] southWestNorthEast = new Vector2[2];
        float rW     = width * 0.5f;
        float rH     = height * 0.5f;
        float xWest  = Point.x - rW;
        float xEast  = Point.x + rW;
        float ySouth = Point.y - rH;
        float yNorth = Point.y + rH;
        return new Vector2[2] { new Vector2 (xWest, ySouth), new Vector2 (xEast, yNorth) };
    }

}
