using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    private List<Vector2Int> TilePoints;
    private NodeController currentNode;
    private int currentTileIndex;
    private NodeCollectionController.ExitingDirection? currentExitDirection;

    public Path(List<Vector2Int> tilePoints)
    {
        TilePoints = tilePoints;
        if (tilePoints.Count < 1) throw new System.Exception("Destination already reached");
        currentNode = GetStartingNodeFromPath(tilePoints[0], tilePoints[1]);
        currentTileIndex = 0;
        TryUpdateExitingDirection();
    }

    /// <summary>
    /// Takes the first two tile positions on the grid and returns the spawn position from the first tile.
    /// </summary>
    private NodeController GetStartingNodeFromPath(Vector2Int firstPosition, Vector2Int secondPosition)
    {
        var delta = (secondPosition - firstPosition);
        var enteringDirection = NodeCollectionController.GetEnteringFromDelta(delta);

        if(!TryGetCollectionAtPosition(firstPosition, out NodeCollectionController collection)) throw new Exception("Tile position not valid");
        return collection.GetSpawnNode(enteringDirection);
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
        if (currentExitDirection is null) return false; //Has reached direction
        currentNode = currentNode.GetNodeByDirection(currentExitDirection.Value);
        if(currentNode == null) 
        {
            return TryAdvanceNextTile();
        }
        return true;
    }

    private bool TryAdvanceNextTile()
    {
        currentTileIndex++;
        if (ReachedDestination()) return false;
        else
        {
            //If able to get the node collection controller at tile position
            if (TryGetCollectionAtPosition(TilePoints[currentTileIndex], out NodeCollectionController collection))
            {
                var entering = (NodeCollectionController.EnteringDirection)currentExitDirection;
                currentNode = collection.GetInboundNode(entering);
                TryUpdateExitingDirection();

                return true;
            }
            else
            {
                throw new Exception("Cant find tile at that position"); //TODO : Remove this. Only for debug, should just return false
                return false;
            }
        }
    }

    private bool TryUpdateExitingDirection()
    {
        if (currentTileIndex + 1 <= TilePoints.Count) //Is not on the last tile
        {
            var delta = TilePoints[currentTileIndex + 1] - TilePoints[currentTileIndex]; //How to handle the OOB Exception?
            currentExitDirection = NodeCollectionController.GetExitingFromDelta(delta);
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

    private bool ReachedDestination() => currentTileIndex >= TilePoints.Count;
}
