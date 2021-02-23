using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PlaceSensorState<T> : DrawsWithCursorState where T : Component
{
    public override void OnMouseDown(DigitalCursor location)
    {
        if (location.OnGrid && SensorManager.TryCreateSensorAt<T>(location.Position))
        {
            ClickCursor();
        }
    }
}
