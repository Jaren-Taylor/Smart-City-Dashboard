using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PixelPathGraph
{
    Dictionary<Vector2Int, HashSet<Vector2Int>> graphData = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

    public void AddNode(Vector2Int position)
    {
        graphData.Add(position, new HashSet<Vector2Int>());
    }

    public void ConnectNodes(Vector2Int node1, Vector2Int node2)
    {
        if(graphData.TryGetValue(node1, out var n1Connections) && graphData.TryGetValue(node2, out var n2Connections))
        {
            if (n1Connections.Contains(node2) || n2Connections.Contains(node1)) throw new System.Exception("Connection already present");

            n1Connections.Add(node2);
            n2Connections.Add(node1);
        }
        else throw new System.Exception("Node does not exist");
    }

    public bool IsConnected(Vector2Int node1, Vector2Int node2)
    {
        return graphData.TryGetValue(node1, out var n1Connections) &&
            graphData.TryGetValue(node2, out var n2Connections) &&
            n1Connections.Contains(node2) &&
            n2Connections.Contains(node1);
    }

    public bool IsNodeKnown(Vector2Int position)
    {
        return graphData.ContainsKey(position);
    }
}
