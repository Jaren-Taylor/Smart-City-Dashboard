using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    private List<Vector2Int> TilePoints;
    private NodeController currentNode;
    private int currentTile;
    private NodeCollectionController.ExitingDirection currentExitDirection;

    public Path(List<Vector2Int> tilePoints)
    {
        TilePoints = tilePoints;
        if (tilePoints.Count < 1) throw new System.Exception("Destination already reached"); 
        currentNode = GetStartingNodeFromPath(tilePoints[0], tilePoints[1]);
        currentTile = 0;
    }

    public Path(List<Vector2Int> tilePoints, NodeController initalNode)
    {
        TilePoints = tilePoints;
        currentNode = initalNode;
        currentTile = 0;
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
    /// Advances current node to the next one along the path. Returns false if at end of path or unable to advance
    /// </summary>
    /// <returns></returns>
    public bool AdvanceNextNode()
    {
        currentNode = currentNode.GetNodeByDirection(currentExitDirection);
        if(currentNode == null)
        {
            return TryAdvanceNextTile();
        }
        return false;
    }

    private bool TryAdvanceNextTile()
    {
        currentTile++;
        if (ReachedDestination()) return false;
        else
        {
            if (TryGetCollectionAtPosition(TilePoints[currentTile], out NodeCollectionController collection))
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

    private bool TryUpdateExitingDirection() //Will fail if entering last tile. 
    {
        if (!ReachedDestination())
        {
            var delta = TilePoints[currentTile + 1] - TilePoints[currentTile];
            currentExitDirection = NodeCollectionController.GetExitingFromDelta(delta);
            return true;
        }
        return false;
    }

    private bool TryGetCollectionAtPosition(Vector2Int position, out NodeCollectionController collection)
    {
        collection = GridManager.Instance.GetCollectionAtTileLocation(position);
        if (collection is null) return false;
        return true;
    }

    private bool ReachedDestination() => currentTile >= TilePoints.Count;
}
