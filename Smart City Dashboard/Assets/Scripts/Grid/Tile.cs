using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public abstract class Tile
{
    public Action<Tile> OnTileDestroyed;

    public enum Facing
    {
        Left = 0,
        Right = 1,
        Top = 2,
        Bottom = 3
    }

    public static Facing OppositeDirection(Facing direction)
    {
        if (direction == Facing.Left) return Facing.Right;
        else if (direction == Facing.Right) return Facing.Left;
        else if (direction == Facing.Top) return Facing.Bottom;
        else return Facing.Top;
    }

    public static readonly Vector2Int[] Directions = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };
    public static readonly Tile.Facing[] Facings = { Facing.Left, Facing.Right, Facing.Top, Facing.Bottom };


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
    protected ManagedGameObject managedObject = null;

    [DataMember(Name="IsPermanent")]
    public bool IsPermanent { get; private set; }

    //[DataMember(Name="IsTransparent")]
    [IgnoreDataMember]
    private bool isTransparent = true;

    [DataMember(Name="Sensors")]
    private HashSet<SensorType> sensors = new HashSet<SensorType>();

    public IReadOnlyCollection<SensorType> Sensors => sensors.ToList().AsReadOnly();

    [IgnoreDataMember]
    private Dictionary<SensorType, GameObject> sensorDictionary = new Dictionary<SensorType, GameObject>();

    public void SetTransparency(bool value)
    {
        if (value) managedObject.SetModelMaterial(GridManager.Instance.TransparentMaterial);
        else managedObject.SetModelMaterial(GridManager.Instance.TileMaterial);
        isTransparent = value;
    }

    private NodeCollectionController cachedCollection = null;


#pragma warning disable UNT0008 // Null propagation on Unity objects
    public NodeCollectionController NodeCollection => cachedCollection ??= GetComponent<PathfindingTileInterface>()?.NodeCollection;
#pragma warning restore UNT0008 // Null propagation on Unity objects

    public Tile(bool isPermanent) { this.IsPermanent = isPermanent; }

    public Tile()
    {
        IsPermanent = false;
        
    }

    public override string ToString()
    {
        return base.ToString() + " (Perm: " + IsPermanent.ToString() + ")";
    }

    public void MakePermanent(bool animateSpawn = false)
    {
        if (!IsPermanent) { 
            IsPermanent = true;
            if (animateSpawn)
            {
                managedObject.AddComponentToManaged<SlideIntoPlace>();
            }
            SpawnManagedNodes();
        }
    }

    public void SpawnHeldSensors()
    {
        if (sensorDictionary is null) sensorDictionary = new Dictionary<SensorType, GameObject>();
        if (sensors is null) sensors = new HashSet<SensorType>();
        foreach(SensorType sensor in sensors)
        {
            if (!sensorDictionary.ContainsKey(sensor))
            {
                SpawnSensor(sensor);
            }
        }
    }

    private bool SpawnSensor(SensorType sensorType)
    {
        if (ManagedExists() && IsSensorAllowed(sensorType))
        {
            GameObject sensorRef = GameObject.Instantiate<GameObject>(sensorType.GetPrefab(), managedObject.transform);
            sensorDictionary.Add(sensorType, sensorRef);
            return true;
        }
        return false;
    }

    protected virtual bool IsSensorAllowed(SensorType sensorType) => false;

    public bool TryAddSensor(SensorType sensorType)
    {
        if (!sensors.Contains(sensorType) && SpawnSensor(sensorType))
        {
            ForceAddSensor(sensorType);
            return true;
        }
        return false;
    }

    public void ForceAddSensor(SensorType sensorType)
    {
        sensors.Add(sensorType);
    }

    public void RemoveSensor(SensorType sensorType)
    {
        if (sensors.Contains(sensorType))
        {
            sensors.Remove(sensorType);
            if (sensorDictionary.TryGetValue(sensorType, out GameObject spawnedSensor))
            {
                GameObject.Destroy(spawnedSensor);
                sensorDictionary.Remove(sensorType);
            }
        }
    }

    public void RemoveSensors()
    {
        foreach (SensorType sensor in sensors) RemoveSensor(sensor);
    }

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
        IsPermanent = false;
        OnTileDestroyed?.Invoke(this);

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
        if (IsPermanent)
        {
            SpawnManagedNodes();
        }
        return CalculateAndSetModelFromNeighbors(neighbors);
    }

    private void SpawnManagedNodes()
    {
        if(!managedObject.TryGetComponent<PathfindingTileInterface>(out _)) managedObject.AddComponent<PathfindingTileInterface>();
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
        managedObject.InstantiateModel(prefabLocation, FacingToQuaternion[direction]);
        
    }
    

    /// <summary>
    /// Given the neighbors and the position, will use decide which model should be created as well a how to orient it.
    /// </summary>
    /// <param name="neighbors">The neighbor information required to determine orientation</param>
    /// <returns>Returns true when this object is flagged for deletion from grid!</returns>
    protected abstract bool CalculateAndSetModelFromNeighbors(NeighborInfo neighbors);
}


public static class FacingExtension
{
    public static Vector2Int ToVector2(this Tile.Facing facing) => facing switch
    {
        Tile.Facing.Right => Vector2Int.right,
        Tile.Facing.Left => Vector2Int.left,
        Tile.Facing.Top => Vector2Int.up,
        Tile.Facing.Bottom => Vector2Int.down,
        _ => throw new Exception()
    };

    public static bool IsOppisite(this Tile.Facing facing, Tile.Facing other) => (facing, other) switch
    {
        (Tile.Facing.Right, Tile.Facing.Left) => true,
        (Tile.Facing.Left, Tile.Facing.Right) => true,
        (Tile.Facing.Top, Tile.Facing.Bottom) => true,
        (Tile.Facing.Bottom, Tile.Facing.Top) => true,
        _ => false,
    };

    public static Tile.Facing Oppisite(this Tile.Facing facing) => facing switch
    {
        Tile.Facing.Right => Tile.Facing.Left,
        Tile.Facing.Left => Tile.Facing.Right,
        Tile.Facing.Top => Tile.Facing.Bottom,
        Tile.Facing.Bottom => Tile.Facing.Top,
        _ => throw new Exception()
    };

    public static Tile.Facing TurnLeft(this Tile.Facing facing) => facing switch
    {
        Tile.Facing.Right => Tile.Facing.Top,
        Tile.Facing.Left => Tile.Facing.Bottom,
        Tile.Facing.Top => Tile.Facing.Left,
        Tile.Facing.Bottom => Tile.Facing.Right,
        _ => throw new Exception()
    };

    public static Tile.Facing TurnRight(this Tile.Facing facing) => facing switch
    {
        Tile.Facing.Right => Tile.Facing.Bottom,
        Tile.Facing.Left => Tile.Facing.Top,
        Tile.Facing.Top => Tile.Facing.Right,
        Tile.Facing.Bottom => Tile.Facing.Left,
        _ => throw new Exception()
    };

    public static Vector2Int DiagonalLeftVector(this Tile.Facing facing) => facing switch
    {
        Tile.Facing.Right => new Vector2Int(1, 1),
        Tile.Facing.Left => new Vector2Int(-1, -1),
        Tile.Facing.Top => new Vector2Int(-1, 1),
        Tile.Facing.Bottom => new Vector2Int(1, -1),
        _ => throw new Exception()
    };

    public static Vector2Int DiagonalRightVector(this Tile.Facing facing) => facing switch
    {
        Tile.Facing.Right => new Vector2Int(1, -1),
        Tile.Facing.Left => new Vector2Int(-1, 1),
        Tile.Facing.Top => new Vector2Int(1, 1),
        Tile.Facing.Bottom => new Vector2Int(-1, -1),
        _ => throw new Exception()
    };

    public static bool IsHorizontal(this Tile.Facing facing) => facing switch
    {
        Tile.Facing.Right => true,
        Tile.Facing.Left => true,
        _ => false
    };
}
