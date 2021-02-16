using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Schema;
using UnityEngine;

public class NodeCollectionController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] NodeCollection;
    //private List<GameObject> nodeLevelPath = new List<GameObject>();
    //public GameObject currentlyOccupiedNode;
    //public GameObject nextNode;
    //public GameObject occupyingEntity;
    //public Tile target;
    //public Tile origin;
    public enum TargetUser
    {
        Pedestrians,
        Vehicles,
        Both // for areas which will occasionally need to be traversed by both entity types
    }
    private class Node
    {
        public int X;
        public int Y;
        public bool Occupied { get => Occupied; set { Occupied = value; } }
        public Vector3 Position { get => Position; set { Position = value; } }
        public TargetUser TargetUser { get => TargetUser; set { TargetUser = value; } }
        public Vector3 Target { get => Target; set => SetDistance(value); }
        public Vector2Int Vector2Pos { get => Vector2Pos; set { Vector2Pos = Vector2Int.RoundToInt(Position); } }
        public int Cost;
        public int Distance { get; private set; }
        public int CostDistance => Cost + Distance;

        public Node Parent;

        public void SetDistance(Vector3 vector3Target)
        {
            Vector2Int target = Vector2Int.RoundToInt(vector3Target);
            this.Distance = Mathf.Abs(target.x - X) + Mathf.Abs(target.y - Y);
        }

        public void SetVector2Int(Vector3 pos)
        {
            Vector2Int.RoundToInt(pos);
        }
    }
     // TODO Build Data Structure for Lanes on road ways, and allow for cross entity traversal on certain nodes.

    //private void Awake()
    //{
    //    foreach (GameObject node in NodeCollection)
    //    {
    //        Debug.Log(Array.IndexOf<GameObject>(NodeCollection, node));
    //    }

    //}

    public GameObject GetNode(int col, int row)
    {
        if(col < 4 & row < 4) return this.NodeCollection[col + row * 4];
        else { throw new IndexOutOfRangeException(); }
    }

    private List<Vector3> FindPathToNextTile(GameObject[] nodeCollection, Vector3 origin, Vector3 destination)
    {
        List<Vector3> directions = new List<Vector3>();
        var start = new Node() { Position = origin };
        var finish = new Node() { Position = destination };

        start.SetDistance(destination);
        var activeNodes = new List<Node>();
        activeNodes.Add(start);
        var visitedNodes = new List<Node>();
        while (activeNodes.Any())
        {
            var checkNode = activeNodes.OrderBy(x => x.CostDistance).First();

            if (checkNode.X == finish.X && checkNode.Y == finish.Y)
            {
                return NodeToVectorList(checkNode);
            }
            visitedNodes.Add(checkNode);
            activeNodes.Remove(checkNode);

            var walkableNodes = GetWalkableNodes(nodeCollection, checkNode, finish);

            foreach (var walkableNode in walkableNodes)
            {
                if (visitedNodes.Any(node => node.X == walkableNode.X && node.Y == walkableNode.Y))
                    continue; 
                if (activeNodes.Any(node => node.X == walkableNode.X && node.Y == walkableNode.Y))
                {
                    var existingNode = activeNodes.First(node => node.X == walkableNode.X && node.Y == walkableNode.Y);

                    if (existingNode.CostDistance > walkableNode.CostDistance) //TODO : Check if this should be checkTile instead
                    { //Switch it out!
                        activeNodes.Remove(existingNode);
                        activeNodes.Add(walkableNode);
                    }
                }
                else
                {
                    activeNodes.Add(walkableNode);
                }
            }
        }
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
}
