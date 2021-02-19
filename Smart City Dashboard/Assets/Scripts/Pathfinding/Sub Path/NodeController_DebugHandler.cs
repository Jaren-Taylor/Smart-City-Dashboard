using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeController))]
public class NodeController_DebugHandler : Editor
{
    private readonly NodeCollectionController.Direction? specificDirection = NodeCollectionController.Direction.EastBound;
    private readonly NodeCollectionController.TargetUser? specificTargetType = NodeCollectionController.TargetUser.Vehicles;

    private void OnSceneGUI()
    {
        NodeController navPT = target as NodeController;

        if (navPT is null || navPT.Connections == null)
            return;

        Vector3 center = navPT.transform.position;
        foreach(Connection connection in navPT.Connections)
        {
            if ((specificDirection.HasValue && specificDirection.Value != connection.Exiting) || //If connection type is not debug type to draw
                (specificTargetType.HasValue && specificTargetType.Value != connection.PathType))  //If connection target is not debug target to draw
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
        NodeCollectionController.Direction.WestBound => Color.blue,
        _ => Color.gray
    };
}
