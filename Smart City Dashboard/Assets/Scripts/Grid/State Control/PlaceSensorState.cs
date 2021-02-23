using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PlaceSensorState : IGridControlState
{
    private CursorIndicator indicator;
    private readonly string indicatorPrefabAddress = "Prefabs/UI/Cursor_Indicator";

    public void OnMouseDown(DigitalCursor location)
    {
        indicator.OnClick();
    }

    public void OnMouseEnterTile(DigitalCursor location)
    {
        indicator.SetPosition(location.Position);
    }

    public void OnMouseExitTile(DigitalCursor location)
    {
        indicator.SetPosition(indicator.OffGrid);
    }

    public void OnPop(DigitalCursor location)
    {
        GameObject.Destroy(indicator.gameObject);
    }

    public void OnPush(DigitalCursor location)
    {
        var prefab = Resources.Load<GameObject>(indicatorPrefabAddress);
        indicator = GameObject.Instantiate(prefab).GetComponent<CursorIndicator>();
        indicator.SetPosition(location.Position);
    }
}
