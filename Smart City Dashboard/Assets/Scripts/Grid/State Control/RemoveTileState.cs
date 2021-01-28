using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GridManager.Instance.RemoveTile(location.Position);
    }

    public void OnMouseEnterTile(DigitalCursor location)
    {
        GridManager.Instance.SetTransparency(location.Position, true);
    }

    public void OnMouseExitTile(DigitalCursor location)
    {
        GridManager.Instance.SetTransparency(location.Position, false);
    }


}
