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

    private Dictionary<NodeCollectionController.Direction, NodeController> VehicleConnectionDictionary;
    private Dictionary<NodeCollectionController.Direction, NodeController> PedestrianConnectionDictionary;

    public Vector3 Position => gameObject.transform.position;

    private void Start()
    {
        (VehicleConnectionDictionary, PedestrianConnectionDictionary) = ConnectionListToDictionarys();
    }

    private (Dictionary<NodeCollectionController.Direction, NodeController> vehicle, Dictionary<NodeCollectionController.Direction, NodeController> pedestrian) ConnectionListToDictionarys()
    {
        Dictionary<NodeCollectionController.Direction, NodeController> vehicle = new Dictionary<NodeCollectionController.Direction, NodeController>();
        Dictionary<NodeCollectionController.Direction, NodeController> pedestrian = new Dictionary<NodeCollectionController.Direction, NodeController>();

        foreach(Connection connection in Connections)
        {
            switch (connection.PathType)
            {
                case NodeCollectionController.TargetUser.Vehicles:
                    vehicle.Add(connection.Exiting, connection.NC);
                    break;
                case NodeCollectionController.TargetUser.Pedestrians:
                    pedestrian.Add(connection.Exiting, connection.NC);
                    break;
                default:
                    vehicle.Add(connection.Exiting, connection.NC);
                    pedestrian.Add(connection.Exiting, connection.NC);
                    break;
            }
        }

        if (vehicle.Keys.Count < 4 || pedestrian.Keys.Count < 4) throw new Exception("");

        return (vehicle, pedestrian);
    }

    public NodeController GetNodeForVehicleByDirection(NodeCollectionController.Direction direction) => VehicleConnectionDictionary[direction];
    public NodeController GetNodeForPedestrianByDirection(NodeCollectionController.Direction direction) => PedestrianConnectionDictionary[direction];

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
