using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GeoDataExtractor;

public class PixelPathGraph
{
    Dictionary<Vector2Int, PixelPathNode> graphData = new Dictionary<Vector2Int, PixelPathNode>();
    Dictionary<Vector2Int, int> pixelData = new Dictionary<Vector2Int, int>();

    /// <summary>
    /// Negative values are local, positive are main (0 is a result of averaging)
    /// </summary>
    public IReadOnlyDictionary<Vector2Int, int> PixelData => pixelData;

    public HashSet<(Vector2Int p1, Vector2Int p2)> GetAllLines()
    {
        HashSet<(Vector2Int, Vector2Int)> output = new HashSet<(Vector2Int, Vector2Int)>();

        foreach(var kvp in graphData)
        {
            foreach(var con in kvp.Value.Connections)
            {
                if(!output.Contains((kvp.Key, con)) && !output.Contains((con, kvp.Key)))
                {
                    output.Add((kvp.Key, con));
                }
            }
        }

        return output;
    }

    public List<Vector2Int> GetPointsOfInterest() => graphData.Where(x => IsNodeOfInterest(x.Key)).Select(x => x.Key).ToList();

    public List<Vector2Int> GetAllPoints() => graphData.Keys.ToList();

    public Dictionary<Vector2Int, PixelPathNode>.KeyCollection GetNodePositions() => graphData.Keys;

    public bool IsNodeOfInterest(Vector2Int position)
    {
        if(!graphData.ContainsKey(position))
        {
            throw new System.Exception();
        }
        return graphData[position].Connections.Count != 2;
    }

    public (HashSet<Vector2Int> connectedPositions, PixelType pixelData) GetNodeData(Vector2Int position) => (graphData[position].Connections, GetPixelType(position));
    

    public void AddNode(Vector2Int position, PixelType type)
    {
        graphData.Add(position, new PixelPathNode());
        pixelData.Add(position, (type == PixelType.MainRoad) ? 1 : -1);
    }

    public void MarkVisited(Vector2Int position)
    {
        graphData[position].isVisited = true;
    }

    public void ClearVisited()
    {
        foreach(var kvp in graphData)
        {
            kvp.Value.isVisited = false;
        }
    }

    public bool IsVisited(Vector2Int position) => graphData[position].isVisited;

    public void CollapseConnection(Vector2Int node1, Vector2Int node2)
    {
        if(IsConnected(node1, node2))
        {
            foreach(var node in graphData[node1].Connections)
            {
                if (node == node2) continue;
                if(!IsConnected(node, node2))
                {
                    graphData[node].Connections.Add(node2);
                    graphData[node2].Connections.Add(node);
                }
                graphData[node].Connections.Remove(node1);
                
            }
            graphData[node2].Connections.Remove(node1);
            pixelData.Remove(node1); //Remove node1 from the graph
            graphData.Remove(node1);
        }
        else throw new System.Exception("Connection not present");
    }

    private void ReplaceConnection(Vector2Int node, Vector2Int currentConnectedNode, Vector2Int newTargetNode)
    {
        if (graphData[node].Connections.Contains(currentConnectedNode))
        {
            graphData[node].Connections.Remove(currentConnectedNode);
            graphData[node].Connections.Add(newTargetNode);
        }
        else throw new System.Exception("No connection to replace");
    }

    public void ConnectNodes(Vector2Int node1, Vector2Int node2)
    {
        if(graphData.TryGetValue(node1, out var n1NodeObj) && graphData.TryGetValue(node2, out var n2NodeObj))
        {
            if (n1NodeObj.Connections.Contains(node2) || n2NodeObj.Connections.Contains(node1))
            {
                throw new System.Exception("Connection already present");

            }

            n1NodeObj.Connections.Add(node2);
            n2NodeObj.Connections.Add(node1);
        }
        else throw new System.Exception("Node does not exist");
    }

    public bool IsConnected(Vector2Int node1, Vector2Int node2)
    {
        return graphData.TryGetValue(node1, out var n1NodeObj) &&
            graphData.TryGetValue(node2, out var n2NodeObj) &&
            n1NodeObj.Connections.Contains(node2) &&
            n2NodeObj.Connections.Contains(node1);
    }

    public HashSet<Vector2Int> GetConnections(Vector2Int position) => graphData[position].Connections;

    public bool TryGetConnections(Vector2Int position, out HashSet<Vector2Int> connections)
    {
        if(graphData.TryGetValue(position, out var node))
        {
            connections = node.Connections;
            return true;
        }
        connections = null;
        return false;
    }

    public PixelType GetPixelType(Vector2Int position) => pixelData[position] < 0 ? PixelType.LocalRoad : PixelType.MainRoad;

    public bool TryGetPixelType(Vector2Int position, out PixelType type)
    {
        if(pixelData.TryGetValue(position, out var intensity))
        {
            type = (intensity < 0) ? PixelType.LocalRoad : PixelType.MainRoad;
            return true;
        }
        type = PixelType.None;
        return false;
    }

    public bool DoesNodeExist(Vector2Int position)
    {
        return graphData.ContainsKey(position);
    }
}

public class PixelPathNode
{
    public HashSet<Vector2Int> Connections = new HashSet<Vector2Int>();
    public bool isVisited = false;
}
