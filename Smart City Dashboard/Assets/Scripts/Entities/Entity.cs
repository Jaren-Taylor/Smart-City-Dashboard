using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity
{
    Vector2Int pos; // Possibly useless

    Vector2Int spawnPos;

    Vector2Int destinationPos;

    private ManagedGameObject managedObject;

    public GameObject Bus_Base  = (GameObject) Resources.Load("Assets/Resources/Prefabs/Vehicles/Bus_Base", typeof(GameObject));

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
       GameObject.Instantiate(
            Bus_Base,
            new Vector3(point.x + .25f, .1f, point.y + .25f),
            Quaternion.identity);
    }
}
