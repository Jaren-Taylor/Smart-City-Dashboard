using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRoadState : IGridControlState
{

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
        if (location != null && GridManager.Instance.GetTile(location.Value) is RoadTile)
        {
            GridManager.Instance.MakePermanent(location.Value);
        }
    }

    public void OnMouseEnterTile(Vector2Int? location)
    {
        if (location != null) GridManager.Instance.CreateTemporaryTile<RoadTile>(location.Value);
    }

    public void OnMouseExitTile(Vector2Int? location)
    {
        if (location != null) GridManager.Instance.RemoveTileIfTemporary(location.Value);
    }
}
