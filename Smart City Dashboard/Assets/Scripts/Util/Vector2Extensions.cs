using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{
    public static float DistanceToLine(this Vector2 point, Vector2 linePoint1, Vector2 linePoint2)
    {
        Vector2 projectedPoint = Vector3.Project(point - linePoint1, linePoint2 - linePoint1);//+ linePoint1;
        projectedPoint += linePoint1;
        return Vector2.Distance(projectedPoint, point);
    }

    public static float SignedDistanceToLine(this Vector2 point, Vector2 linePoint1, Vector2 linePoint2)
    {
        Vector2 projectedPoint = Vector3.Project(point - linePoint1, linePoint2 - linePoint1);//+ linePoint1;
        int sign = (Vector2.SignedAngle(point - projectedPoint, linePoint2 - linePoint1) > 0) ? 1 : -1; 
        projectedPoint += linePoint1;


        return Vector2.Distance(projectedPoint, point) * sign;
    }

    public static Vector2Int RotateAround(this Vector2Int position, float angle, Vector2 center)
    {
        Vector2 delta = position - center;
        Vector2 rotated = (Quaternion.AngleAxis(angle, Vector3.forward) * delta);
        rotated += center;

        return Vector2Int.RoundToInt(rotated);
    }

    public static bool IsCollinearWith(this Vector2Int p1, Vector2Int p2, Vector2Int p3)
    {
        List<float> Distances = new List<float>()
        {
            Vector2.Distance(p1, p2),
            Vector2.Distance(p2, p3),
            Vector2.Distance(p3, p1)
        };

        Distances.Sort();

        return Distances[2].BasicallyEquals(Distances[1] + Distances[0]);
    }

    public static NodeCollectionController.Direction? ToDirection(this Vector2 delta)
    {
        Vector2 flatNorm = delta;
        flatNorm.Normalize();
        if (flatNorm.y > 0.707f) return NodeCollectionController.Direction.NorthBound;
        else if (flatNorm.y < -0.707f) return NodeCollectionController.Direction.SouthBound;
        else if (flatNorm.x > 0.707f) return NodeCollectionController.Direction.EastBound;
        else if (flatNorm.x < -0.707f) return NodeCollectionController.Direction.WestBound;
        else return null;
    }

    public static Vector3 ToGridVector3(this Vector2 position) => new Vector3(position.x, 0f, position.y);
    public static Vector3 ToGridVector3(this Vector2Int position) => new Vector3(position.x, 0f, position.y);
}

