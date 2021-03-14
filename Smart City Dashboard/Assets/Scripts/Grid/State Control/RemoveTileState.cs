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
        if (location.IsValid()) GridManager.Instance.RemoveTile(location.Position);
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
