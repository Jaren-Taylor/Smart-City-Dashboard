using UnityEngine;

class RemoveSensorState : DrawsWithCursorState
{
    public override string GetIconPrefabAddress() => "Prefabs/UI/X_Icon";

    public override void OnMouseDown(DigitalCursor location)
    {
        // TODO: Add way to open sensor menu
        if (location.OnGrid)
        {
            ClickCursor();
        }
    }
}
