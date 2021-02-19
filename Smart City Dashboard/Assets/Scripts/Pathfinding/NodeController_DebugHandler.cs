using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeController))]
public class NodeController_DebugHandler : Editor
{
    private void OnSceneGUI()
    {
        NodeController navPT = target as NodeController;

        if (navPT is null || navPT.Connections == null)
            return;

        Vector3 center = navPT.transform.position;
        foreach(Connection connection in navPT.Connections)
        {
            Color color;
            switch (connection?.Exiting)
            {
                case NodeCollectionController.Direction.EastBound:
                    color = Color.red;
                    break;

                case NodeCollectionController.Direction.SouthBound:
                    color = Color.green;
                    break;

                case NodeCollectionController.Direction.WestBound:
                    color = Color.blue;
                    break;

                default:
                    color = Color.gray;
                    break;

            }

            NavPoint_DebugHandler.DrawDebugArrow(center, connection.NC?.transform.position ?? Vector3.up + center, color, NavPoint_DebugHandler.ArrowType.SingleEnded );
        }
    }

}
