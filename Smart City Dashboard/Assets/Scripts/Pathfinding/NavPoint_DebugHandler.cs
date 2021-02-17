using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavPoint))]
public class NavPoint_DebugHandler : Editor
{
    private void OnSceneGUI()
    {
        NavPoint navPT = target as NavPoint;
        if (navPT.Connections == null) 
            return;

        Vector3 center = navPT.transform.position;
        foreach(NavPointConnection connection in navPT.Connections)
        {
            if (connection.Target == null) continue;
            if (connection.Type == NavPointConnection.ConnectionType.Directed) DrawDebugArrow(center, connection.Target.transform.position, Color.red, ArrowType.SingleEnded);
            else if (connection.Type == NavPointConnection.ConnectionType.Bidirectional) DrawDebugArrow(center, connection.Target.transform.position, Color.blue, ArrowType.DoubleEnded);
        }
    }

    public enum ArrowType { SingleEnded, DoubleEnded } 

    public static void DrawDebugArrow(Vector3 from, Vector3 to, Color? color = null, ArrowType? type = null)
    {
        float lineLength = Vector3.Distance(from, to);
        float radius = Mathf.Clamp(.05f * lineLength, .05f, .1f);
        Vector3 direction = Vector3.Normalize(to - from);

        Color stashedColor = Handles.color;
        type ??= ArrowType.SingleEnded;

        Handles.color = color ?? Color.white;
        Handles.DrawLine(from, to);
        DrawDebugWirePyramid(to - direction * .5f, to, radius);
        if(type == ArrowType.DoubleEnded) DrawDebugWirePyramid(from + direction * .5f, from, radius);
        Handles.color = stashedColor;
    }

    private static void DrawDebugWirePyramid(Vector3 baseCenter, Vector3 tip, float width)
    {
        width = Mathf.Clamp(width, 0.05f, 2f);

        Vector3 pointing = tip - baseCenter;
        if (pointing.magnitude == 0) return;

        Vector3[] basePry = new Vector3[]
        {
            Vector3.up * Mathf.Abs(pointing.magnitude),
            new Vector3(width, 0, width),
            new Vector3(-width, 0, width),
            new Vector3(-width, 0, -width),
            new Vector3(width, 0, -width)
        };

        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, pointing);
        for(int i = 0; i < 5; i++)
        {
            basePry[i] = rotation * basePry[i] + baseCenter; 
        }

        DrawAllPointsConnected(basePry);

    }

    private static void DrawAllPointsConnected(Vector3[] points)
    {
        for(int i = 0; i < points.Length; i++)
        {
            for (int j = i + 1; j < points.Length; j++) 
            {
                Handles.DrawLine(points[i], points[j]);
            }
        }
    }
}
