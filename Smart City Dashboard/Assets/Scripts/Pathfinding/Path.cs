using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    private List<Vector2Int> TilePoints;
    private NodeController currentNode;
    private int currentTileIndex;
    private NodeCollectionController currentCollection;
    private NodeCollectionController.Direction? currentExitDirection;
    private NodeCollectionController.TargetUser userType;


    public Path(List<Vector2Int> tilePoints, NodeController startingPoint, NodeController endingPoint,  NodeCollectionController.TargetUser userType)
    {
        TilePoints = tilePoints;
        this.userType = userType;
        if (tilePoints.Count < 1) throw new System.Exception("Destination already reached");
        currentNode = startingPoint;
        currentTileIndex = 0;
        TryUpdateExitingDirection();
        TryGetCollectionAtPosition(TilePoints[currentTileIndex], out currentCollection);
    }

    /// <summary>
    /// Gets the current node to move to
    /// </summary>
    /// <returns></returns>
    public NodeController GetCurrentNode() => currentNode;

    /// <summary>
    /// Advances current node to the next one along the path. Returns false if already at end of path or unable to advance.
    /// </summary>
    /// <returns></returns>
    public bool AdvanceNextNode()
    {
        //Once current exit direcition is null, this indicates entity is on the final tile.
        if (currentExitDirection is null) return false;

        //Debug.Log($"Move toward: [{currentExitDirection}]");

        //Figures out the next node on the current collection
        var nextNode = GetNodeInDirection(currentExitDirection.Value);

        //Debug.Log(nextNode?.Position);

        //If next node was found to be null, try to transition to next tile
        if(nextNode is null) return TryAdvanceNextTile();

        //If next node was found in current collection, overwrite current node with new one
        else currentNode = nextNode;
        
        //Return successfully able to find next node
        return true;
    }

    private NodeController GetNodeInDirection(NodeCollectionController.Direction direction) => 
        (userType == NodeCollectionController.TargetUser.Vehicles) ?
            currentNode.GetNodeForVehicleByDirection(direction) :
            currentNode.GetNodeForPedestrianByDirection(direction);

    private bool TryAdvanceNextTile()
    {
        //Point tile index to the next tile
        currentTileIndex++;

        //Checks if there is no next tile to find
        if (ReachedDestinationTile()) return false;
        else
        {
            //Debug.Log("Advancing Next node");

            int position = currentCollection.GetPositionFrom(currentExitDirection.Value, currentNode);

            //If able to get the node collection controller at tile position, updates current collection controller
            if (TryGetCollectionAtPosition(TilePoints[currentTileIndex], out currentCollection))
            {

                //Debug.Log($"Entering from: [{currentExitDirection},{position}]");

                //Depending on the position and exiting direction sets node to the adjacent node on the next collection
                currentNode = currentCollection.GetInboundNodeFrom(currentExitDirection.Value, position);

                //Trys to get new exiting direction. Will fail when entering the last tile
                TryUpdateExitingDirection();

                //Debug.Log($"Updated exiting: [{currentExitDirection}]");

                return true;
            }
            else
            {
                Debug.Log(GridManager.Instance.Grid);
                throw new Exception("Cant find tile at that position"); //TODO : Remove this. Only for debug, should just return false
                return false;
            }
        }
    }

    private bool TryUpdateExitingDirection()
    {
        if (currentTileIndex + 1 < TilePoints.Count) //Is not on the last tile
        {
            //How to handle the OOB Exception?
            currentExitDirection = NodeCollectionController.GetDirectionFromDelta(TilePoints[currentTileIndex + 1], TilePoints[currentTileIndex]);
            return true;
        }
        currentExitDirection = null;
        return false;
    }

    private bool TryGetCollectionAtPosition(Vector2Int position, out NodeCollectionController collection)
    {
        collection = GridManager.Instance.GetCollectionAtTileLocation(position);
        if (collection is null) return false;
        return true;
    }

    private bool ReachedDestinationTile() => currentTileIndex >= TilePoints.Count;
}
