using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class VehicleEntity : Entity
{
    public Vector3 position;
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

    [DataMember(Name = "Type")]
    public VehicleType Type { get; private set; }
    public VehicleEntity(Vector3 position)
    {
        this.position = position;
    }


    public override string ToString()
    {
        return "[Vehicle]: " + base.ToString();
    }

    public VehicleEntity()
    {

    }
    
}
