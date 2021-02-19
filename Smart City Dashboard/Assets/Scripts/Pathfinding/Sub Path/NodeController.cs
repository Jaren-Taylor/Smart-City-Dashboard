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

    private Dictionary<NodeCollectionController.Direction, NodeController> ConnectionDictionary;

    public Vector3 Position => gameObject.transform.position;

    private void Start()
    {
        ConnectionDictionary = ConnectionListToDictionary();
    }

    private Dictionary<NodeCollectionController.Direction, NodeController> ConnectionListToDictionary()
    {
        Dictionary<NodeCollectionController.Direction, NodeController> output = new Dictionary<NodeCollectionController.Direction, NodeController>();
        foreach (Connection con in Connections)
        {
            output.Add(con.Exiting, con.NC);
        }
        return output;
    }

    public NodeController GetNodeByDirection(NodeCollectionController.Direction direction)
    {
        if (ConnectionDictionary.TryGetValue(direction, out NodeController next)) return next;
        else return null;
    }


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
    public NodeCollectionController.Direction Exiting;
    [SerializeField]
    public NodeController NC;
    [SerializeField]
    public NodeCollectionController.TargetUser PathType;
}
