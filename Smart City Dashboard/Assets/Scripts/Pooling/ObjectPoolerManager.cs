using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolerManager : MonoBehaviour
{
    public static Pool<PedestrianEntity> PedestrianPool = new Pool<PedestrianEntity>();
    public static Pool<VehicleEntity> VehiclePool = new Pool<VehicleEntity>();
    public static ObjectPoolerManager Instance;

    private void Start()
    {
        Instance = this;
    }

    public static void ClearPools()
    {
        for (int i = 0; i < PedestrianPool.Loaned.Count; i++)
        {
            Destroy(PedestrianPool.Loan());
        }
        PedestrianPool.Clear();

        for (int i = 0; i < VehiclePool.Loaned.Count; i++)
        {
            Destroy(VehiclePool.Loan());
        }
        VehiclePool.Clear();
    }
}
