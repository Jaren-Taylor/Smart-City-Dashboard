using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRoadState : IGridControlState
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
        if (location != null && GridManager.Instance.GetTile(location.Position) is RoadTile)
        {
            GridManager.Instance.MakePermanent(location.Position);
        }
    }

    public void OnMouseEnterTile(DigitalCursor location)
    {
        if (location != null) GridManager.Instance.CreateTemporaryTile<RoadTile>(location.Position);
    }

    public void OnMouseExitTile(DigitalCursor location)
    {
        if (location != null) GridManager.Instance.RemoveTileIfTemporary(location.Position);
    }
}
