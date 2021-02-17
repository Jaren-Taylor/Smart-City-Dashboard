using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity
{
    Vector2Int pos; // Possibly useless

    Vector2Int spawnPos;

    Vector2Int destinationPos;

    private ManagedGameObject managedObject;
    private const string ManagedGameObjectLocation = "Prefabs/ManagedEntity";

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

    public Vector3 InstantiateEntity(Vector2Int point)
    {
        Vector3 position = new Vector3(point.x + .2f, .1f, point.y + .2f);
        return InstantiateEntity(position);

    }

    public Vector3 InstantiateEntity(Vector3 point)
    {
        managedObject = Object.Instantiate(
             Resources.Load<ManagedGameObject>(ManagedGameObjectLocation),
             point,
             Quaternion.identity);
        AttachModelToManaged("Prefabs/Vehicles/Bus_Base");
        return point;

    }
    protected void AttachModelToManaged(string prefabLocation)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabLocation);
        managedObject.InstantiateModel(prefab, Quaternion.Euler(-90, -90, -90));
    }

    /// <summary>
    /// Add component to the object it is managing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddComponent<T>() where T : Component => managedObject.AddComponent<T>();

    /// <summary>
    /// Removes component from the object it is managing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool RemoveComponent<T>() where T : Component => managedObject.TryRemoveComponent<T>();

    /// <summary>
    /// Trys to remove component from the object it is managing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool TryGetComponent<T>(out T component) where T : Component
    {
        component = GetComponent<T>();
        return component != null;
    }

    /// <summary>
    /// Gets component from the object it is managing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetComponent<T>() where T : Component => managedObject?.GetComponent<T>();
}
