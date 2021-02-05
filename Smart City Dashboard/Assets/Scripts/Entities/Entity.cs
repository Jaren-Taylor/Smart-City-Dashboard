using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity
{
    Vector2Int pos; // Possibly useless

    Vector2Int spawnPos;

    Vector2Int destinationPos;

    private ManagedGameObject managedObject;
    private const string ManagedGameObjectLocation = "Prefabs/ManagedTile";

    //public Vector2Int requestTilePos(Vector2Int tilePos)
    //{

    //}
    public Entity()
    {
        this.managedObject = null;
    }

    public Entity(Vector2Int pos, ManagedGameObject model)
    {
        this.spawnPos = pos;
        this.managedObject = model;
    }

    public Entity(Vector2Int pos, Vector2Int destination, ManagedGameObject model)
    {

        this.spawnPos = pos;
        this.destinationPos = destination;
        this.managedObject = model;

    }

    public void InstantiateEntity(Vector2Int point)
    {
       managedObject = Object.Instantiate(
            Resources.Load<ManagedGameObject>(ManagedGameObjectLocation),
            new Vector3(point.x + .2f, .1f, point.y + .35f),
            Quaternion.identity);
        AttachModelToManaged("Prefabs/Vehicles/Bus_Base");
    }
    protected void AttachModelToManaged(string prefabLocation)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabLocation);
        managedObject.InstantiateModel(prefab, Quaternion.Euler(-90, -90, -90));
    }
}
