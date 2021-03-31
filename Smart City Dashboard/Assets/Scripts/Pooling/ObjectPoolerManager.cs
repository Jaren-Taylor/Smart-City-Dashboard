using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolerManager : MonoBehaviour
{
    public static Dictionary<Type, dynamic> Pools = new Dictionary<Type, dynamic>();
    private static ObjectPoolerManager instance;

    private void Start()
    {
        instance = this;
        Pools.Add(typeof(VehicleEntity), new Pool<PedestrianEntity>());
        Pools.Add(typeof(PedestrianEntity), new Pool<VehicleEntity>());
    }

    public static void ReclaimEntity(Entity entity)
    {
        if (Pools.ContainsKey(entity.GetType())) throw new System.Exception("Cannot reclaim entity. There is no existing pool for this entity type");
        Pools[entity.GetComponentInChildren<Entity>().GetType()].Reclaim(entity);
        entity.PreviousDestinations.Add(entity.gameObject.transform.position.ToGridInt());
        entity.transform.position = Vector3.zero;
        entity.GetComponent<PathWalker>().Path = null;
        entity.transform.SetParent(instance.transform);
        entity.gameObject.SetActive(false);
    }

    public static Entity GetEntityFromPool(Type type)
    {
        var entity = Pools[type].Loan();
        entity.transform.SetParent(GameObject.Find("EntityManager").transform);
        entity.transform.position = entity.GetComponent<PathWalker>().SpawnPosition.Position;
        entity.gameObject.SetActive(true);
        return (Entity)entity;
    }

    private void ReclaimAllLoaned()
    {
        foreach (var key in Pools.Keys)
        {
            foreach (var poolable in Pools[key].loaned)
            {
                ReclaimEntity((Entity)poolable);
            }
        }
    }

    public static void ClearPools()
    {
        foreach (var key in Pools.Keys)
        {
            for (int i = 0; i < Pools[key].count; i++)
            {
                Destroy(Pools[key].Loan());
            }
            Pools[key].Clear();
        }
    }
}
