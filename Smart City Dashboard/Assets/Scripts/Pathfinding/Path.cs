using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    private List<Vector2Int> TilePoints;
    private NodeController currentNode;
    private int currentTileIndex;
    private NodeCollectionController.Direction? currentExitDirection;

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
        var enteringDirection = NodeCollectionController.GetDirectionFromDelta(firstPosition, secondPosition);

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
            Debug.Log("Trying to advance to next tile" + currentExitDirection.ToString());
            return TryAdvanceNextTile();
        }
        Debug.Log(currentNode.Position);
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
                //Casting a null to a direction returns the first item in the enumeration
                currentNode = collection.GetInboundNode(currentExitDirection.Value);
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

    private bool ReachedDestination() => currentTileIndex >= TilePoints.Count;
}
