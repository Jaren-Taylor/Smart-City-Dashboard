using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;


public class VehicleEntity : Entity
{
    public enum VehicleType{
        Car,
        Bus,
        Van,
        Bike
    }

    public static readonly Dictionary<VehicleType, string> ModelLookup = new Dictionary<VehicleType, string>()
    {
        { VehicleType.Bus, "Prefabs/Vehicles/Bus_Base" },
        { VehicleType.Van, "Prefabs/Vehicles/Van_Base" }
        
    };
    public static VehicleEntity Spawn(Vector2Int tilePosition, VehicleType type)
    {
        var address = ModelLookup[type];
        var model = Resources.Load<GameObject>(address);
        Tile tile = GridManager.Instance.Grid[tilePosition];
        NodeCollectionController.Direction spawnDirection = GetValidDirectionForTile(tile);
        //Hi Kenny
        var NCC = tile.GetComponent<PathfindingNodeInterface>();
        NodeController spawnLocation = NCC.NodeCollection.GetSpawnNode(spawnDirection);
        var vehicleEntity = Instantiate(model, spawnLocation.Position, Quaternion.identity).GetComponent<VehicleEntity>();
        vehicleEntity.SpawnPosition = spawnLocation;
        return vehicleEntity;
    }

    private static NodeCollectionController.Direction GetValidDirectionForTile(Tile tile)
    {
        if (tile is BuildingTile building)
        {
            return (NodeCollectionController.Direction)building.currentFacing;
        }
        else if (tile is RoadTile road)
        {
            return GetValidRoadDirection(road);
        }
        throw new System.Exception("Invalid Tile Type...HOW?");
    }

    //TODO: Decide which side of the road to spawn on
    private static NodeCollectionController.Direction GetValidRoadDirection(RoadTile road)
    {
        return NodeCollectionController.Direction.EastBound;
    }

    public VehicleType Type { get; private set; }
}
