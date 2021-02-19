using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeController))]
public class NodeController_DebugHandler : Editor
{
    private readonly NodeCollectionController.Direction? specificDirection = NodeCollectionController.Direction.NorthBound;
    private readonly NodeCollectionController.TargetUser? specificTargetType = NodeCollectionController.TargetUser.Vehicles;

    private void OnSceneGUI()
    {
        NodeController nodePT = target as NodeController;

        if (nodePT is null || nodePT.Connections == null)
            return;

        DrawDebugArrows(nodePT, specificDirection, specificTargetType);
    }

    public static void DrawDebugArrows(NodeController nodePT, NodeCollectionController.Direction? forceDirection, NodeCollectionController.TargetUser? forceTargetType)
    {
        Vector3 center = nodePT.transform.position;
        foreach (Connection connection in nodePT.Connections)
        {
            if ((forceDirection.HasValue && forceDirection.Value != connection.Exiting) || //If connection type is not debug type to draw
                (forceTargetType.HasValue && forceTargetType.Value != connection.PathType && connection.PathType != NodeCollectionController.TargetUser.Both))  //If connection target is not debug target to draw
                continue; //Skip drawing this connection

            Color color = GetColorFromDirection(connection.Exiting);

            Vector3 target = connection.NC?.transform.position ?? Vector3.up + center;

            NavPoint_DebugHandler.DrawDebugArrow(center, target, color, NavPoint_DebugHandler.ArrowType.SingleEnded);
        }
    }


    /// <summary>
    /// Uses pattern matching to get color from the direction
    /// </summary>
    private static Color GetColorFromDirection(NodeCollectionController.Direction direction) => 
    direction switch
    {
        NodeCollectionController.Direction.EastBound => Color.red,
        NodeCollectionController.Direction.SouthBound => Color.green,
        NodeCollectionController.Direction.WestBound => Color.yellow,
        _ => Color.blue
    };
}
