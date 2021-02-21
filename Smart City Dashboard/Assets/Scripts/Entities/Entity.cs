using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public NodeController SpawnPosition { get; protected set; }
    private float maxSpeed = .5f;
    private Path path;
    public Vector2Int TilePosition => Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.z));
    public Action<Entity, float> OnReachedDestination;
    protected bool TrySetDestination(Vector2Int tileLocation, NodeCollectionController.TargetUser targetUser)
    {
        var pathList = Pathfinding.GetListOfPositionsFromTo(TilePosition, tileLocation);
        if (pathList is null)
            return false;
        path = new Path(pathList, SpawnPosition, null, targetUser);
        return !(path is null);
    }

    /// <summary>
    /// Spawns Entity of type T on node from the address specificied
    /// </summary>
    protected static T Spawn<T>(NodeController spawnNode, string prefabAddress) where T : Entity
    {
        var model = Resources.Load<GameObject>(prefabAddress);
        var entity = Instantiate(model, spawnNode.Position, Quaternion.identity).GetComponent<T>();
        entity.SpawnPosition = spawnNode;
        return entity;
    }

    /// <summary>
    /// Spawns Entity of type T on tile position from the address specificied
    /// </summary>
    protected static T Spawn<T>(Vector2Int tilePosition, string prefabAddress) where T : Entity
    {
        Tile tile = GridManager.Instance.Grid[tilePosition];
        NodeCollectionController.Direction spawnDirection = GetValidDirectionForTile(tile);
        var NCC = tile.GetComponent<PathfindingTileInterface>();
        NodeController spawnLocation = NCC.NodeCollection.GetInboundNodeFrom(spawnDirection, 2);

        //Uses location found to spawn prefab
        return Spawn<T>(spawnLocation, prefabAddress);
    }

    private static NodeCollectionController.Direction GetValidDirectionForTile(Tile tile)
    {
        if (tile is BuildingTile building)
        {
            return (NodeCollectionController.Direction)building.currentFacing;
        }
        else if (tile is RoadTile road)
        {
            return GetValidRoadDirection(road);
        }
        throw new System.Exception("Invalid Tile Type...HOW?");
    }

    //TODO: Decide which side of the road to spawn on
    private static NodeCollectionController.Direction GetValidRoadDirection(RoadTile road)
    {
        return NodeCollectionController.Direction.EastBound;
    }

    private void Update()
    {
        if (HasPath())
        {
            MoveAlongPath();
        }
    }
    //Comment
    private void MoveAlongPath()
    {
        if (path.GetCurrentNode() is NodeController nodeController)
        {
            transform.LookAt(nodeController.transform.position);
            MoveTowardsPosition(nodeController.transform.position);
            if (HasArrivedAtNode(nodeController.transform.position))
            {
                if (!path.AdvanceNextNode())
                {
                    DestroyPath(2f);
                }
            }
        }
        else DestroyPath(0f);
    }

    private bool HasArrivedAtNode(Vector3 position) => Vector3.Distance(transform.position, position) < .0005;
    private void MoveTowardsPosition(Vector3 position) => transform.position = Vector3.MoveTowards(transform.position, position, maxSpeed * Time.deltaTime);
    private void DestroyPath(float delay)
    {
        path = null;
        OnReachedDestination?.Invoke(this, delay);
    }
    private bool HasPath() => !(path is null);

    public abstract bool TrySetDestination(Vector2Int tileLocation);
}