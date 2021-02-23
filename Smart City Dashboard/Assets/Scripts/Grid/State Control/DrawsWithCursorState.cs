using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DrawsWithCursorState : IGridControlState
{
    private CursorIndicator indicator;
    private readonly string indicatorPrefabAddress = "Prefabs/UI/Cursor_Indicator";

    public void ClickCursor()
    {
        indicator.OnClick();
    }

    public abstract void OnMouseDown(DigitalCursor location);

    public virtual void OnMouseEnterTile(DigitalCursor location)
    {
        indicator.SetPosition(location.Position);
    }

    public virtual void OnMouseExitTile(DigitalCursor location)
    {
        indicator.SetPosition(indicator.OffGrid);
    }

    public virtual void OnPop(DigitalCursor location)
    {
        GameObject.Destroy(indicator.gameObject);
    }

    public virtual void OnPush(DigitalCursor location)
    {
        var prefab = Resources.Load<GameObject>(indicatorPrefabAddress);
        indicator = GameObject.Instantiate(prefab).GetComponent<CursorIndicator>();
        indicator.SetPosition(location.Position);
    }
}
