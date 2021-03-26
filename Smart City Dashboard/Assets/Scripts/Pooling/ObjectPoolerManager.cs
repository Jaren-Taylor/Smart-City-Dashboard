using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolerManager : MonoBehaviour
{
    private static List<PedestrianEntity> PooledPedestrianEntityList = new List<PedestrianEntity>();
    private static List<VehicleEntity> PooledVehicleEntityList = new List<VehicleEntity>();
    private static HashSet<Entity> LoanedEntities = new HashSet<Entity>();

    public static void FillPools()
    {
        for(int i = 0; i < 1000; i++)
        {
            var entity = VehicleEntity.Spawn(new Vector2Int(0,0), (VehicleEntity.VehicleType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(VehicleEntity.VehicleType)).Length));
            entity.gameObject.SetActive(false);
            PooledVehicleEntityList.Add(entity);
        }
        for (int i = 0; i < 1000; i++)
        {
            var entity = PedestrianEntity.Spawn(new Vector2Int(0,0));
            entity.gameObject.SetActive(false);
            PooledPedestrianEntityList.Add(entity);
        }

    }

    public static void ReclaimObject(Entity entity)
    {
        entity.gameObject.SetActive(false);
        entity.transform.position = Vector3.zero;
        if (entity.GetComponentInChildren<VehicleEntity>() is VehicleEntity)
        {
            PooledVehicleEntityList.Add((VehicleEntity)entity);
        }

        if (entity.GetComponentInChildren<PedestrianEntity>() is PedestrianEntity)
        {
            PooledPedestrianEntityList.Add((PedestrianEntity)entity);
        }

    }
    public static bool CanLoanVehicle() => (PooledVehicleEntityList.Count > 0);
    public static bool CanLoanPedestrian() => (PooledPedestrianEntityList.Count > 0);

    public static VehicleEntity GetVehicleEntityFromPool()
    {
        var vehicle = PooledVehicleEntityList[PooledVehicleEntityList.Count - 1];
        PooledVehicleEntityList.RemoveAt(PooledVehicleEntityList.Count - 1);
        LoanedEntities.Add(vehicle);
        return vehicle;
    }

    public static PedestrianEntity GetPedestrianEntityFromPool()
    {
        var pedestrian = PooledPedestrianEntityList[PooledPedestrianEntityList.Count - 1];
        PooledPedestrianEntityList.RemoveAt(PooledPedestrianEntityList.Count - 1);
        LoanedEntities.Add(pedestrian);
        return pedestrian;
    }

    private void ReclaimAllLoanedObjects()
    {
        foreach(Entity entity in LoanedEntities)
        {
            entity.gameObject.SetActive(false);

            if(entity.GetComponentInChildren<VehicleEntity>() is VehicleEntity)
            {
                PooledVehicleEntityList.Add((VehicleEntity)entity);
            }

            if(entity.GetComponentInChildren<PedestrianEntity>() is PedestrianEntity)
            {
                PooledPedestrianEntityList.Add((PedestrianEntity)entity);
            }

        }
        LoanedEntities.Clear();
    }
}
