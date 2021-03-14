using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamineTileSensorsState : DrawsWithCursorState
{
    public override string GetIconPrefabAddress()
    {
        return "";
    }

    public override void OnMouseDown(DigitalCursor location)
    {
        if (location.IsValid() && GridManager.GetTile(location.Position) is Tile)
        {
            UIManager.Instance.InspectTile(location.Position);
        }
    }
}
