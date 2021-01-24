using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTile : Structure
{
    public enum StructureType
    {
        house = 0,
        office = 1
    }

    public StructureType structure;
}
