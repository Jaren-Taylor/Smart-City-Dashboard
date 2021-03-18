using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;


[DataContract]
/// <summary>
/// Data structure to manage placing buildings
/// </summary>
public class BuildingTile : Tile
{
    public static readonly Dictionary<StructureType, string> ModelLookup = new Dictionary<StructureType, string>()
    {
        { StructureType.House, "Prefabs/Structures/House" },
        { StructureType.Office, "Prefabs/Structures/Office" },
        { StructureType.TestStruct, "Prefabs/Structures/TestStruct" }
    };

    public enum StructureType
    {
        House = 0,
        Office = 1,
        TestStruct = 2
    }

    [DataMember(Name="Structure")]
    public readonly StructureType structure;
    [DataMember(Name="Facing")]
    public Facing currentFacing { get; private set; }
    [DataMember(Name="IsLocationValid")]
    public bool IsLocationValid { get; private set; }

    public BuildingTile(StructureType type, Facing facing, bool isPerm) : base(isPerm)
    {
        structure = type;
        currentFacing = facing;
    }

    public BuildingTile(StructureType type, Facing facing)
    {
        structure = type;
        currentFacing = facing;
    }

    public override string ToString()
    {
        return $"[{structure.ToString()}]: " +  base.ToString();
    }

    /// <summary>
    /// Determines which directions are valid for this object to be placed. Then calls back to parent to create the file.
    /// If this returns true, it needs to be garbage collected.
    /// </summary>
    /// <param name="neighbors"></param>
    /// <returns></returns>
    protected override bool CalculateAndSetModelFromNeighbors(NeighborInfo neighbors)
    {
        var validDirection = new List<Facing>(GetValidDirections(neighbors, structure));
        if(validDirection.Contains(currentFacing))
        {
            if (ModelExist())
            {
                IsLocationValid = true;
                return false;
            }
            else
            {
                IsLocationValid = true;
                //Use the current facing
            }
        }
        else
        {
            if (validDirection.Count == 0)
            {
                IsLocationValid = false;
                currentFacing = Facing.Bottom;
            } else
            {
                IsLocationValid = true;
                currentFacing = validDirection[0];
            }
        }
        AttachModelToManaged(ModelLookup[structure], currentFacing); //Tells parent to construct the model in the orientation 
        return IsPermanent && !IsLocationValid;
    }

    private IEnumerable<Facing> GetValidDirections(NeighborInfo neighbors, StructureType structure)
    {
        switch (structure)
        {
            default:
                if (neighbors.left is RoadTile) yield return Facing.Left;
                if (neighbors.right is RoadTile) yield return Facing.Right;
                if (neighbors.top is RoadTile) yield return Facing.Top;
                if (neighbors.bottom is RoadTile) yield return Facing.Bottom;
                break;
        }
    }
}
