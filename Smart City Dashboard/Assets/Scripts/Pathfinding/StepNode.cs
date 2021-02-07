using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class StepNode 
{
    private List<StepNode> neighbors = new List<StepNode>();
    private StepNode next = null;
    public Vector3 pos { get; }
    private string directions = "lrud";

   
    public StepNode()
    {
        neighbors = null;
    }

    public StepNode(Vector3 pos, List<StepNode> adjacentNeighbors, StepNode next=null)
    {
        this.neighbors = adjacentNeighbors;
        this.next = next;
        this.pos = pos;
    }

    private void consumeDirections(string directions)
    {
        char[] map = directions.ToCharArray();
        foreach( char step in map)
        {

        }
    }
}
