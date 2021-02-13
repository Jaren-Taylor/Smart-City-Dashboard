using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public abstract class Tile
{
    public enum Facing
    {
        Left = 0,
        Right = 1,
        Top = 2,
        Bottom = 3
    }

    public static readonly Vector2Int[] Directions = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

    public static Facing VectorToFacing(Vector2 delta)
    {
        Vector2 normalized = delta.normalized;
        float bestDot = -1;
        Facing bestFacing = Facing.Left;
        for (int i = 0; i < 4; i++)
        {
            float dot = Vector2.Dot(normalized, Directions[i]);
            if(dot > bestDot)
            {
                bestDot = dot;
                bestFacing = (Facing)i;
            }
        }

        return bestFacing;

    }

    public readonly static Dictionary<Facing, Quaternion> FacingToQuaternion = new Dictionary<Facing, Quaternion>()
    {
        { Facing.Top, Quaternion.identity },
        { Facing.Bottom, Quaternion.Euler(0, 180, 0) },
        { Facing.Right, Quaternion.Euler(0, 90, 0) },
        { Facing.Left, Quaternion.Euler(0, -90, 0) }
    };

    private const string ManagedGameObjectLocation = "Prefabs/ManagedTile";
    private ManagedGameObject managedObject = null;

    [DataMember(Name="IsPermanent")]
    public bool IsPermanent { get; private set; }

    [DataMember(Name="IsTransparent")]
    private bool isTransparent = true;

    public void SetTransparency(bool value)
    {
        if (value) managedObject.SetModelMaterial(GridManager.Instance.TransparentMaterial);
        else managedObject.SetModelMaterial(GridManager.Instance.TileMaterial);
        isTransparent = value;
    }

    public Tile()
    {
        IsPermanent = false;
    }

    public override string ToString()
    {
        return base.ToString() + " (Perm: " + IsPermanent.ToString() + ")";
    }

    public void MakePermanent() => IsPermanent = true;

    /// <summary>
    /// Check if the structure has a GameObject model instantiated
    /// </summary>
    /// <returns>True if GameObject model is instantiated.</returns>
    public bool ManagedExists() => managedObject != null;

    public bool ModelExist() => (ManagedExists()) ? false : managedObject.ModelExists;

    /// <summary>
    /// Deletes managedObject along with it's children
    /// </summary>
    /// <returns></returns>
    public void DeleteManaged()
    {
        managedObject?.DestroyTree();
        managedObject = null;
    }

    /// <summary>
    /// Initalizes the managed structure and determines how to placed prefabs appropriately. Deletes existing model if it exists.
    /// </summary>
    /// <param name="point">Location to generate the model at</param>
    /// <param name="neighbors">The neighbor information required to determine orientation</param>
    /// <returns>Returns true when this object is flagged for deletion from grid!</returns>
    public bool CreateManaged(Vector2Int point, NeighborInfo neighbors)
    {
        if (ManagedExists()) DeleteManaged();
        managedObject = GameObject.Instantiate(
            Resources.Load<ManagedGameObject>(ManagedGameObjectLocation),
            new Vector3(point.x, 0, point.y),
            Quaternion.identity,
            GridManager.Instance?.transform);
        managedObject.name = managedObject.name.Replace("(Clone)", "");
        SetTransparency(isTransparent);
        return CalculateAndSetModelFromNeighbors(neighbors);
    }


    /// <summary>
    /// Updates managed object to match surrounding. Throws error if managed is not initalized or is has been marked for deletion
    /// </summary>
    /// <param name="neighbors">The neighbor information required to determine placement</param>
    /// <returns>Returns true when this object is flagged for deletion from grid!</returns>
    public bool RecalculateManaged(NeighborInfo neighbors)
    {
        if (!ManagedExists()) throw new UnassignedReferenceException("Model has not yet been initilized. Call create manage first!");
        return CalculateAndSetModelFromNeighbors(neighbors);

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


    /// <summary>
    /// Used by subcalsses to attach their tile model to the managed game object
    /// </summary>
    /// <param name="prefabLocation">Folder location to load resource from</param>
    /// <param name="rotation">Rotation to spawn model at</param>
    protected void AttachModelToManaged(string prefabLocation, Facing direction)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabLocation);
        managedObject.InstantiateModel(prefab, FacingToQuaternion[direction]);
    }

    /// <summary>
    /// Given the neighbors and the position, will use decide which model should be created as well a how to orient it.
    /// </summary>
    /// <param name="neighbors">The neighbor information required to determine orientation</param>
    /// <returns>Returns true when this object is flagged for deletion from grid!</returns>
    protected abstract bool CalculateAndSetModelFromNeighbors(NeighborInfo neighbors);
}
