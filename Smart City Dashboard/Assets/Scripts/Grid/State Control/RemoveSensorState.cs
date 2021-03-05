using UnityEngine;

class RemoveSensorState : DrawsWithCursorState
{
    public override string GetIconPrefabAddress() => "Prefabs/UI/X_Icon";

    public override void OnMouseDown(DigitalCursor location)
    {
        if (location.OnGrid && SensorManager.TryRemoveSensorsAt<CameraSensor>(location.Position))
        {
            ClickCursor();
        }
    }
}
