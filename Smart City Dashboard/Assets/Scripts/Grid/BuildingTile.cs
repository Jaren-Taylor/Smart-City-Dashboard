using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTile : Tile
{
    public static readonly Dictionary<StructureType, string> ModelLookup = new Dictionary<StructureType, string>()
    {
        { StructureType.House, "Prefabs/Structures/House" },
        { StructureType.TestStruct, "Prefabs/Structures/TestStruct" }
    };

    public enum StructureType
    {
        House = 0,
        TestStruct = 1
    }

    public readonly StructureType structure;
    private Facing currentFacing;
    public bool IsLocationValid { get; private set; }



    public BuildingTile(StructureType type, Facing facing)
    {
        structure = type;
        currentFacing = facing;
    }

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
        AttachModelToManaged(ModelLookup[structure], currentFacing);
        return false;
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
