using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Schema;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeCollectionController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] NodeCollection;

    private List<Vector3> VehicleExits = new List<Vector3>();
    private List<Node> NodeStructure = new List<Node>();
    public enum TargetUser
    {
        Pedestrians,
        Vehicles,
        Both // for areas which will occasionally need to be traversed by both entity types
    }
    public enum ExitingDirection
    {
        NorthBound,
        EastBound,
        SouthBound,
        WestBound
    }

    public static ExitingDirection GetExitingFromDelta(Vector2Int delta)
    {
        //Get direction from movement delta
        if (delta.x == 1 && delta.y == 0) return ExitingDirection.EastBound;
        else if (delta.x == -1 && delta.y == 0) return ExitingDirection.WestBound;
        else if (delta.x == 0 && delta.y == 1) return ExitingDirection.NorthBound;
        else if (delta.x == 0 && delta.y == -1) return ExitingDirection.SouthBound;
        else throw new Exception("Path invalid"); //Delta is wonky
    }

    public enum EnteringDirection
    {
        NorthBound,
        EastBound,
        SouthBound,
        WestBound
    }

    public static EnteringDirection GetEnteringFromDelta(Vector2Int delta)
    {
        //Get direction from movement delta
        if (delta.x == 1 && delta.y == 0) return EnteringDirection.EastBound;
        else if (delta.x == -1 && delta.y == 0) return EnteringDirection.WestBound;
        else if (delta.x == 0 && delta.y == 1) return EnteringDirection.NorthBound;
        else if (delta.x == 0 && delta.y == -1) return EnteringDirection.SouthBound;
        else throw new Exception("Path invalid"); //Delta is wonky
    }


    private class PedestrianNode
    {

    }
    private class Node
    {
        public int Col;
        public int Row;
        public Tuple<int,int> index { get => index; set => new Tuple<int, int>(Col, Row); }
        public bool Occupied { get => Occupied; set { Occupied = value; } }
        public Vector3 Position { get => Position; set { Position = value; } }
        public EnteringDirection EnteringDirection { get; set; }
        public ExitingDirection ExitingDirection { get; set; }
        public TargetUser TargetUser { get => TargetUser; set { TargetUser = value; } }
        public Vector3 TargetNode { get; set; }

        public Node Parent;
    }
    [Serializable]
    public class TileConnection { }

    // TODO Build Data Structure for Lanes on road ways, and allow for cross entity traversal on certain nodes.

    private void Awake()
    {
        NodeController inbound = GetInboundNode(EnteringDirection.SouthBound);


    }

    public GameObject GetNode(int row, int col)
    {
        if(col < 4 & row < 4) return this.NodeCollection[col + row * 4];
        else { throw new IndexOutOfRangeException(); }
    }

    public NodeController GetSpawnNode(EnteringDirection direction)
    {
        return GetInboundNode(direction); //Temp fix
    }

    public NodeController GetInboundNode(EnteringDirection direction)
    {
        switch (direction)
        {
            case EnteringDirection.NorthBound:
                return GetNode(0, 2).GetComponent<NodeController>();
            case EnteringDirection.EastBound:
                return GetNode(1, 0).GetComponent<NodeController>();
            case EnteringDirection.WestBound:
                return GetNode(2, 3).GetComponent<NodeController>();
            default:
                return GetNode(3, 1).GetComponent<NodeController>();
        }
    }

    private List<Vector3> FindPathToNextTile(GameObject[] nodeCollection, Vector3 origin, Vector3 destination)
    {
        List<Vector3> directions = new List<Vector3>();
        var start = new Node() { Position = origin };
        var finish = new Node() { Position = destination };

        var activeNodes = new List<Node>();
        activeNodes.Add(start);
        var visitedNodes = new List<Node>();
      
        
        throw new System.Exception("No node-level path found!");
    }
    private static List<Vector3> NodeToVectorList(Node endNode)
    {
        if (endNode == null) return new List<Vector3>();
        Stack<Vector3> output = new Stack<Vector3>();
        Node node = endNode;
        do
        {
            output.Push(node.Position);
            node = node.Parent;
        } while (node != null);

        return output.ToList();
    }
    private static List<Node> GetWalkableNodes(GameObject[] nodeCollection, Node curr, Node target)
    {
        List<Node> nodes = new List<Node>();
        // Get the enum of the node we're currently on, if it's a vehicle it can only follow nodes which are in it's enumeration type (implement lanes and switching later)
        return nodes;
    }
    //private static List<Node> GetViableNeighbors(GameObject[] NodeCollection, Node node)
    //{
    //    List<Node> viableNodes = new List<Node>();
    //    foreach()
    //}
}
