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

    public void OnPop(Vector2Int? location)
    {
        OnMouseExitTile(location);
    }

    public void OnPush(Vector2Int? location)
    {
        OnMouseEnterTile(location);
    }

    public void OnMouseDown(Vector2Int? location)
    {
        if (location != null)
        {
            Tile tile = GridManager.Instance.GetTile(location.Value);
            if(isValidToPlace(tile)) GridManager.Instance.MakePermanent(location.Value);
        }
    }

    private bool isValidToPlace(Tile tile)
    {
        return tile is BuildingTile &&
            ((BuildingTile)tile).structure == Structure &&
            ((BuildingTile)tile).IsLocationValid;
    }

    public void OnMouseEnterTile(Vector2Int? location)
    {
        if (location != null) GridManager.Instance.AddTileToGrid(location.Value, new BuildingTile(Structure));
    }

    public void OnMouseExitTile(Vector2Int? location)
    {
        if (location != null) GridManager.Instance.RemoveTileIfTemporary(location.Value);
    }


}