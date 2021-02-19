using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(NodeCollectionController))]
public class NodeCollectionController_DebugHandler : Editor
{
    private readonly NodeCollectionController.Direction? specificDirection = NodeCollectionController.Direction.NorthBound;
    private readonly NodeCollectionController.TargetUser? specificTargetType = NodeCollectionController.TargetUser.Vehicles;

    private void OnSceneGUI()
    {
        NodeCollectionController ncc = target as NodeCollectionController;
        if (ncc is null) return;

        foreach(NodeController nodePT in ncc.NodeCollectionReference)
        {
            if (nodePT is null || nodePT.Connections == null)
                continue;

            NodeController_DebugHandler.DrawDebugArrows(nodePT, specificDirection, specificTargetType);
            
        }
    }
}
