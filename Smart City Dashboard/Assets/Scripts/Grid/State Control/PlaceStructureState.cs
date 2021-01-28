using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceStructureState : IGridControlState
{
    public readonly BuildingTile.StructureType Structure;

    public PlaceStructureState(BuildingTile.StructureType structure)
    {
        Structure = structure;
    }

    public void OnPop(DigitalCursor location)
    {
        OnMouseExitTile(location);
    }

    public void OnPush(DigitalCursor location)
    {
        OnMouseEnterTile(location);
    }

    public void OnMouseDown(DigitalCursor location)
    {
        if (location != null && location.OnGrid)
        {
            Tile tile = GridManager.Instance.GetTile(location.Position);
            if(isValidToPlace(tile)) GridManager.Instance.MakePermanent(location.Position);
        }
    }

    private bool isValidToPlace(Tile tile)
    {
        return tile is BuildingTile &&
            ((BuildingTile)tile).structure == Structure &&
            ((BuildingTile)tile).IsLocationValid;
    }

    public void OnMouseEnterTile(DigitalCursor location)
    {
        if (location != null) GridManager.Instance.AddTileToGrid(location.Position, new BuildingTile(Structure, location.SubDirection));
    }

    public void OnMouseExitTile(DigitalCursor location)
    {
        if (location != null) GridManager.Instance.RemoveTileIfTemporary(location.Position);
    }


}