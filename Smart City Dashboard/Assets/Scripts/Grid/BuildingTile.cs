using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTile : Tile
{
    public enum StructureType
    {
        house = 0,
        office = 1
    }

    public StructureType structure;

    protected override bool CalculateAndSetModelFromNeighbors(NeighborInfo neighbors)
    {
        throw new System.NotImplementedException();
    }
}
