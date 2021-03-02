using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private PathWalker pathWalker;

    public Action<Entity, float> OnReachedDestination;

    [SerializeField]
    private GameObject Visuals;

    public Action OnBeingDestroy;

    public Vector2Int TilePosition => Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.z));

    [SerializeField]
    private Renderer childRenderer;
    public Material ChildMaterial => childRenderer.material;


    /// <summary>
    /// Sets the destination to the tile specified for the target specified
    /// </summary>
    protected bool TrySetDestination(Vector2Int tileLocation, NodeCollectionController.TargetUser targetUser) => pathWalker.TrySetDestination(tileLocation, targetUser);

    /// <summary>
    /// Spawns Entity of type T on node from the address specificied
    /// </summary>
    protected static T Spawn<T>(NodeController spawnNode, string prefabAddress, string matAddress="") where T : Entity
    {
        var model = Resources.Load<GameObject>(prefabAddress);
        var entityGO = Instantiate(model, spawnNode.Position, Quaternion.identity);
        var entity = entityGO.GetComponent<T>();
        if (matAddress!=""){
            var loadedMat = Resources.Load<Material>(matAddress);
            entity.SetMaterial(loadedMat);
        }
        entity.pathWalker = entityGO.GetComponent<PathWalker>();
        entity.tag = "Entity";
        entity.pathWalker.SpawnPosition = spawnNode;
        entity.pathWalker.OnReachedDestination += entity.ReachedEndOfPath;
        return entity;
    }

    /// <summary>
    /// Called when the path walker has reached the end of the path
    /// </summary>
    /// <param name="delay">How long should wait until destroy</param>
    private void ReachedEndOfPath(float delay) => OnReachedDestination?.Invoke(this, delay);

    /// <summary>
    /// Spawns Entity of type T on tile position from the address specificied
    /// </summary>
    protected static T Spawn<T>(Vector2Int tilePosition, string prefabAddress, string matAddress="") where T : Entity
    {
        Tile tile = GridManager.GetTile(tilePosition);
        NodeCollectionController.Direction spawnDirection = GetValidDirectionForTile(tile);
        NodeController spawnLocation = tile.NodeCollection.GetInboundNodeFrom(spawnDirection, 2);

        //Uses location found to spawn prefab
        return Spawn<T>(spawnLocation, prefabAddress, matAddress);
    }

    /// <summary>
    /// Gets a direction to spawn the entity facing based on the tile it spawns on
    /// </summary>
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

    /// <summary>
    /// Gets the direction to spawn the car based on the fact that it is on a road
    /// </summary>
    //TODO: Decide which side of the road to spawn on
    private static NodeCollectionController.Direction GetValidRoadDirection(RoadTile road)
    {
        return NodeCollectionController.Direction.EastBound;
    }

    private void OnDestroy()
    {
        OnBeingDestroy?.Invoke();
    }

    public void SetModelVisibility(bool value)
    {
        if(Visuals is GameObject)
        {
            Visuals.SetActive(value);
        }
    }

    protected void SetMaterial(Material material)
    {
        childRenderer.material=material;

    }

    /// <summary>
    /// True if the destination was successfully set to the target tile
    /// </summary>
    /// <param name="tileLocation"></param>
    /// <returns></returns>
    public abstract bool TrySetDestination(Vector2Int tileLocation);
}