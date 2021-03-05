using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;


public class VehicleEntity : Entity
{
    public enum VehicleType{
        Car,
        Bus,
        Van
    }

    public static readonly Dictionary<VehicleType, string> ModelLookup = new Dictionary<VehicleType, string>()
    {
        { VehicleType.Bus, "Prefabs/Vehicles/Bus_Base" },
        { VehicleType.Van, "Prefabs/Vehicles/Van_Base" },
        { VehicleType.Car, "Prefabs/Vehicles/Car_Base" }
    };

    public static readonly Dictionary<VehicleColor, string> ColorLookup = new Dictionary<VehicleColor, string>()
    {
        {VehicleColor.Red, "Materials/CarColors/Car_Red" },
        {VehicleColor.Blue, "Materials/CarColors/Car_Blue" },
        {VehicleColor.Black, "Materials/CarColors/Car_Black" },
        {VehicleColor.White, "Materials/CarColors/Car_White" },
        {VehicleColor.Gray, "Materials/CarColors/Car_Gray" },
        {VehicleColor.Silver, "Materials/CarColors/Car_Silver" },
        {VehicleColor.Brown, "Materials/CarColors/Car_Brown" },
        {VehicleColor.Gold, "Materials/CarColors/Car_Gold" },
        {VehicleColor.Green, "Materials/CarColors/Car_Green" },
        {VehicleColor.Turquoise, "Materials/CarColors/Car_Turquoise" },
        {VehicleColor.Orange, "Materials/CarColors/Car_Orange" },
    };

    public static readonly Dictionary<VehicleColor, int> ColorWeight = new Dictionary<VehicleColor, int>()
    {
        {VehicleColor.Black, 23 },
        {VehicleColor.White, 19 },
        {VehicleColor.Gray, 18 },
        {VehicleColor.Silver, 15 },
        {VehicleColor.Red, 9 },
        {VehicleColor.Blue, 10 },
        {VehicleColor.Brown, 2 },
        {VehicleColor.Gold, 2 },
        {VehicleColor.Green, 1 },
        {VehicleColor.Turquoise, 1 },
        {VehicleColor.Orange, 1 },
    };

    public enum VehicleColor {
        Red,
        Blue,
        Black,
        White,
        Gray,
        Silver,
        Brown,
        Gold,
        Green,
        Turquoise,
        Orange
    }

    public static VehicleEntity Spawn(Vector2Int tilePosition, VehicleType type)
    {
        var matAddress = GetWeightedRandomColor();
        var address = ModelLookup[type];
        return Spawn<VehicleEntity>(tilePosition, address, matAddress);
    }
    public static VehicleEntity Spawn(NodeController controller, VehicleType type)
    {
        var matAddress = GetWeightedRandomColor();
        var address = ModelLookup[type];
        return Spawn<VehicleEntity>(controller, address, matAddress);
    }

    private static string GetRandomColor() {
        int count=ColorLookup.Count;
        int choice=UnityEngine.Random.Range(0,count);
        return ColorLookup[(VehicleColor)choice];
    }
    private static string GetWeightedRandomColor()
    {
        var sumOfWeights = ColorWeight.Values.Sum();
        int choice = UnityEngine.Random.Range(0, sumOfWeights);
        
        foreach (var color in ColorWeight.Keys)
        {
            if (choice < ColorWeight[color])
                return ColorLookup[color];
            choice -= ColorWeight[color];
        }
        throw new System.Exception("Invalid Weights");

    }

    public override bool TrySetDestination(Vector2Int tileLocation) => TrySetDestination(tileLocation, NodeCollectionController.TargetUser.Vehicles);

    public VehicleType Type { get; private set; }
}
