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
        { VehicleType.Van, "Prefabs/Vehicles/Van_Base" },
        { VehicleType.Car, "Prefabs/Vehicles/Car_Base" }
    };

    public static VehicleEntity Spawn(Vector2Int tilePosition, VehicleType type)
    {
        var address = ModelLookup[type];
        return Spawn<VehicleEntity>(tilePosition, address);
    }
    public static VehicleEntity Spawn(NodeController controller, VehicleType type)
    {
        var address = ModelLookup[type];
        return Spawn<VehicleEntity>(controller, address);
    }

    public override bool TrySetDestination(Vector2Int tileLocation) => TrySetDestination(tileLocation, NodeCollectionController.TargetUser.Vehicles);

    public VehicleType Type { get; private set; }
}
