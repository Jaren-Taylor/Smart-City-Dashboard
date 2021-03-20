using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier
{
    public static Vector3 PointAlongCurve(List<Vector3> points, float tStep)
    {
        var count = points.Count;

        if (count < 0) throw new System.Exception("List empty");
        if (count == 1) return points[0];
        else return (1 - tStep) * PointAlongCurve(points.GetRange(0, count - 1), tStep) + tStep * PointAlongCurve(points.GetRange(1, count - 1), tStep);
    }

    private static Vector3 PointAlongCurveRec(List<Vector3> points, int startInclusive, int endExclusive, float tStep)
    {
        var count = endExclusive - startInclusive;

        if (count <= 1) return points[startInclusive];
        else return (1 - tStep) * PointAlongCurveRec(points, startInclusive, endExclusive - 1, tStep) + tStep * PointAlongCurveRec(points, startInclusive + 1, endExclusive, tStep); 
    }
}
