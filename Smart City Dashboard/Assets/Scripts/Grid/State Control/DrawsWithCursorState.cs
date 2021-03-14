using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DrawsWithCursorState : IGridControlState
{
    private CursorIndicator indicator = null;
    private readonly string indicatorPrefabAddress = "Prefabs/UI/Cursor_Indicator";

    public void ClickCursor()
    {
        indicator.OnClick();
    }

    public abstract void OnMouseDown(DigitalCursor location);
    public abstract string GetIconPrefabAddress();

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
        indicator?.SetPosition(indicator.OffGrid);
    }

    public virtual void OnPush(DigitalCursor location)
    {
        if (indicator is null)
        {
            var prefab = Resources.Load<GameObject>(indicatorPrefabAddress);
            indicator = GameObject.Instantiate(prefab).GetComponent<CursorIndicator>();
            indicator.SetIcon(GetIconPrefabAddress());
        }
        indicator.SetPosition(location.Position);
    }
}
