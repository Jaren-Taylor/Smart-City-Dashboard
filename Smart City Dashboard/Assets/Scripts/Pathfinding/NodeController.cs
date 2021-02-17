using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NodeController : MonoBehaviour
{
    public List<Connection> Connections = null;
    private float DEBUG_radius = .1f;
    private void OnDrawGizmos()
    {
        Color stashedColor = Gizmos.color;
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, DEBUG_radius);

        Gizmos.color = stashedColor;
    }

    private Dictionary<NodeCollectionController.ExitingDirection, NodeController> ConnectionDictionary;

    private void Start()
    {
        ConnectionDictionary = ConnectionListToDictionary();
    }

    private Dictionary<NodeCollectionController.ExitingDirection, NodeController> ConnectionListToDictionary()
    {
        Dictionary<NodeCollectionController.ExitingDirection, NodeController> output = new Dictionary<NodeCollectionController.ExitingDirection, NodeController>();
        foreach (Connection con in Connections)
        {
            output.Add(con.Exiting, con.NC);
        }
        return output;
    }

    public NodeController GetNodeByDirection(NodeCollectionController.ExitingDirection direction) => ConnectionDictionary[direction];


    private void OnDrawGizmosSelected()
    {
        Color stashedColor = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, DEBUG_radius);

        Gizmos.color = stashedColor;
    }

}

[Serializable]
public class Connection {
    [SerializeField]
    public NodeCollectionController.ExitingDirection Exiting;
    [SerializeField]
    public NodeController NC;
}
