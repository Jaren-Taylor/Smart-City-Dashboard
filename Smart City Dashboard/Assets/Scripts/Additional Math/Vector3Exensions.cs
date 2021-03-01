using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Exensions 
{
    public static bool IsBasicallyEqualTo(this Vector3 value, Vector3 other) => Vector3.Distance(value, other).IsBasicallyZero();
    public static bool IsCollinearWith(this Vector3 p3, Vector3 p1, Vector3 p2)
    {
        List<float> Distances = new List<float>()
        {
            Vector3.Distance(p1, p2),
            Vector3.Distance(p2, p3),
            Vector3.Distance(p3, p1)
        };

        Distances.Sort();

        return Distances[2].BasicallyEquals(Distances[1] + Distances[0]);
    }

    public static Vector2Int ToGridInt(this Vector3 position) => Vector2Int.RoundToInt(new Vector2(position.x, position.z));
    public static Vector2 ToGrid(this Vector3 position) => new Vector2(position.x, position.z);
}
