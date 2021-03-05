using UnityEngine;

class PlaceSensorState<T> : DrawsWithCursorState where T : Component
{
    public override string GetIconPrefabAddress() => "Prefabs/UI/Camera_Icon";

    public override void OnMouseDown(DigitalCursor location)
    {
        if (location.OnGrid && SensorManager.TryCreateSensorAt<T>(location.Position))
        {
            ClickCursor();
        }
    }
}
