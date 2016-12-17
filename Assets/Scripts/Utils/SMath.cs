using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class SMath
{
    public static Vector2[] CreatePlaneFromPoint (Vector2 CentrePoint, float width, float height)
    {
        Vector2[] southWestNorthEast = new Vector2[2];
        float rW     = width * 0.5f;
        float rH     = height * 0.5f;
        float xWest  = CentrePoint.x - rW;
        float xEast  = CentrePoint.x + rW;
        float ySouth = CentrePoint.y - rH;
        float yNorth = CentrePoint.y + rH;
        return new Vector2[2] { new Vector2 (xWest, ySouth), new Vector2 (xEast, yNorth) };
    }

    public static Vector2[] GetDiscretePlanePointsFromCentrePoint (Vector2 CentrePoint, float planeWidth, float planeHeight, uint numDivisions)
    {
        int numPoints = (int) Mathf.Pow (numDivisions + 2.0f, 2.0f) + (numDivisions == 0 ? 1 : 0); // need to add the centre point if the points only include the four planar corners
        float incrementX = planeWidth  / (numDivisions == 0 ? 1 : numDivisions);
        float incrementY = planeHeight / (numDivisions == 0 ? 1 : numDivisions);

        Vector2[] discretePlanePoints = new Vector2[numPoints];
        Vector2[] southWestNorthEast = CreatePlaneFromPoint (CentrePoint, planeWidth, planeHeight);

        bool oddNumPoints = numPoints % 2 == 1;

        // begin in the centre of the plane and loop clockwise starting west, expanding out to fill the points array.
        // filling the array this way means that it can be indexed from 0 - N following the loop, from the centre out.
        Vector2 currentPosition = CentrePoint;
        int pointIndex = 0;
        discretePlanePoints[pointIndex]  = currentPosition;

        currentPosition.x -= incrementX * (oddNumPoints ? 1.0f : 0.5f);
        currentPosition.y += incrementY * (oddNumPoints ? 1.0f : 0.5f);

        uint numLoops = (numDivisions / 2) + (oddNumPoints ? (uint)1 : 0);

        for (int loopIndex = 0; loopIndex < numLoops; ++loopIndex)
        { 
            int numIncrementsInCurrentLoop = (loopIndex + 1) * 2 - (oddNumPoints ? 0 : 1);
            Vector2 loopStartingPosition  = currentPosition;
            for (int direction = 0; direction < 4; ++direction)
            { 
                for (int p = 0; p < numIncrementsInCurrentLoop; ++p)
                {
                    switch (direction)
                    {
                        case 0: currentPosition.x += incrementX; break; // loop east
                        case 1: currentPosition.y -= incrementY; break; // loop south
                        case 2: currentPosition.x -= incrementX; break; // loop west
                        case 3: currentPosition.y += incrementY; break; // loop north
                    }
                    discretePlanePoints[++pointIndex] = currentPosition;
                }
            }
            currentPosition = loopStartingPosition;
            currentPosition.x -= incrementX;
            currentPosition.y += incrementY;
        }

        return discretePlanePoints;
    }
}
