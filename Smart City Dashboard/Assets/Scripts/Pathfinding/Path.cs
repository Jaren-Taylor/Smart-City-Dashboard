using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path
{
    private List<Vector2Int> TilePoints;
    private NodeController currentNode;
    private NodeController endingNode;
    private int currentTileIndex;
    private NodeCollectionController currentCollection;
    private NodeCollectionController.Direction? currentExitDirection;
    private NodeCollectionController.TargetUser userType;


    public Path(List<Vector2Int> tilePoints, NodeController startingPoint, NodeController endingPoint,  NodeCollectionController.TargetUser userType)
    {
        InitalizeClass(tilePoints, startingPoint, endingPoint, userType);
    }

    private void InitalizeClass(List<Vector2Int> tilePoints, NodeController startingPoint, NodeController endingPoint, NodeCollectionController.TargetUser userType)
    {
        TilePoints = tilePoints;
        this.userType = userType;
        endingNode = endingPoint;
        if (TilePoints.Count < 1) throw new System.Exception("Destination already reached");
        currentNode = startingPoint;
        currentTileIndex = 0;
        TryUpdateExitingDirection();
        TryGetCollectionAtPosition(TilePoints[currentTileIndex], out currentCollection);
        RegisterToPath(tilePoints);
    }

    ~Path()
    {
        for(int i = currentTileIndex; i < TilePoints.Count; i++)
        {
            DeregisterFromTileAt(TilePoints[currentTileIndex]);
        }
    }

    private void TileDestroyedOnPath(Tile tile)
    {
        DeregisterFromAll();
        var newPath = Pathfinding.GetListOfPositionsFromTo(TilePoints[currentTileIndex], TilePoints[TilePoints.Count - 1]);
        if (newPath is null)
        {
            // Can't set destination, so destroy self
            currentNode = null;
            endingNode = null;
        }
        else
        {
            // New path valid, assign path object
            InitalizeClass(newPath, currentNode, endingNode, userType);
        }

    }

    public void DeregisterFromAll()
    {
        foreach (Vector2Int tilePos in TilePoints) DeregisterFromTileAt(tilePos);
    }

    public void DeregisterFromTileAt(Vector2Int position)
    {
        if (GridManager.Instance.Grid[position] is Tile t) t.OnTileDestroyed -= TileDestroyedOnPath;
    }

    public void RegisterToPath(List<Vector2Int> tilePoints)
    {
        foreach(Vector2Int tilePos in tilePoints)
        {
            GridManager.GetTile(tilePos).OnTileDestroyed += TileDestroyedOnPath;
        }
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
        //Point tile index to the next tile, and handles deregistration
        DepartCurrentTile();

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
                return false;
            }
        }
    }

    private void DepartCurrentTile()
    {
        DeregisterFromTileAt(TilePoints[currentTileIndex]);
        currentTileIndex++;
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
