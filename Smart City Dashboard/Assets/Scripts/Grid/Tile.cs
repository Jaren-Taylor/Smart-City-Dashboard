using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile
{
    public enum Facing
    {
        Left,
        Right,
        Top,
        Bottom
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

    public bool IsPermanent { get; private set; }

    private bool isTransparent = true;

    private bool attachCalled = false;

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
        SetTransparency(true);
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

    public T AddComponent<T>() where T : Component => managedObject.AddComponent<T>();
    public bool RemoveComponent<T>() where T : Component => managedObject.TryRemoveComponent<T>();
    public bool TryGetComponent<T>(out T component) where T : Component 
    {
        component = GetComponent<T>();
        return component != null;
    }
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
