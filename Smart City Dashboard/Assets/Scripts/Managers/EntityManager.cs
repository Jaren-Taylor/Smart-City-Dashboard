
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    private List<Vector2Int> BuildingLocations => GridManager.Instance.Grid.GetBuildingLocations();
    private readonly List<Entity> Entities = new List<Entity>();
    private int spawnLimit = 5000;

    private void Start()
    {
        
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if(GenerateStartStop(out Vector2Int spawnLocation, out Vector2Int targetLocation))
            {
                NodeCollectionController collection = GridManager.GetTile(spawnLocation).NodeCollection;

                VehicleEntity vehicleEntity = SpawnVehicle(VehicleEntity.VehicleType.Car, collection.GetNode(0, 0));
                vehicleEntity.TrySetDestination(targetLocation);
            }
        }
        if (Input.GetKey(KeyCode.X))
        {
            if(GenerateStartStop(out Vector2Int spawnLocation, out Vector2Int targetLocation))
            {
                NodeCollectionController collection = GridManager.GetTile(spawnLocation).NodeCollection;
                PedestrianEntity pedestrianEntity = SpawnPedestrian(collection.GetNode(0, 0));
                pedestrianEntity.TrySetDestination(targetLocation);
            }
        }

    }
    public PedestrianEntity SpawnPedestrian(NodeController controller)
    {
        PedestrianEntity entity = PedestrianEntity.Spawn(controller);
        Register(entity);
        return entity;
    }
    public VehicleEntity SpawnVehicle(VehicleEntity.VehicleType type, NodeController controller)
    {
        VehicleEntity entity = VehicleEntity.Spawn(controller, type);
        Register(entity);
        return entity;
    }
    private void Register(Entity entity)
    {
        if (Entities.Contains(entity))
            return;

        if (Entities.Count >= spawnLimit)
            HandleOverCapacity();

        entity.transform.SetParent(transform, true);
        Entities.Add(entity);
        entity.OnReachedDestination += StartDespawnCoroutine;

    }

    private void HandleOverCapacity()
    {
        DestroyEntity(Entities[0]);
    }
    private bool GenerateStartStop(out Vector2Int spawnLocation, out Vector2Int targetLocation)
    {
        var buildings = BuildingLocations;
        if (buildings.Count > 0)
        {
            var randIndex = UnityEngine.Random.Range(0, buildings.Count);
            spawnLocation = buildings[randIndex];
            if (buildings.Count > 1)
            {
                buildings.RemoveAt(randIndex);
                targetLocation = buildings[UnityEngine.Random.Range(0, buildings.Count)];
                return true;
            }

        }
        spawnLocation = Vector2Int.zero;
        targetLocation = Vector2Int.zero;
        return false;
    }
    private void DestroyEntity(Entity entity)
    {
        if (Entities.Contains(entity))
        {
            Destroy(entity.gameObject);
            Entities.Remove(entity);
        }
    }
    IEnumerator DestroyAfterDelay(Entity entity, float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyEntity(entity);
    }
    private void StartDespawnCoroutine(Entity entity, float delay)
    {
        StartCoroutine(DestroyAfterDelay(entity, delay));
    }
}
    
