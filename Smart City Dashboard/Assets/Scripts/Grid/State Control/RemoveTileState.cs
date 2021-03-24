using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Removes tile upon clicking. Shows tile to be deleted by making it transparent
/// </summary>
public class RemoveTileState : IGridControlState
{
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
        if (location.IsValid() && GridManager.GetTile(location.Position) is Tile tile && tile.IsPermanent)
        {
            GridManager.Instance.RemoveTile(location.Position);
            if (tile is RoadTile) GridManager.Instance.Grid.RoadRemoved(location.Position);
        }
    }

    public void OnMouseEnterTile(DigitalCursor location)
    {
        if (location.IsValid()) GridManager.Instance.SetTransparency(location.Position, true);
    }

    public void OnMouseExitTile(DigitalCursor location)
    {
        if (location.IsValid()) GridManager.Instance.SetTransparency(location.Position, false);
    }


}
