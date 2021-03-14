using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Upon creation will get mouse cursor information.
/// All fields are readonly and this structure should be discarded after use. 
/// </summary>
public class DigitalCursor
{
    public readonly static int GroundMask = 1 << 6;

    public readonly bool OnGrid;
    public readonly Vector2 HitPosition;
    public readonly Vector2Int Position;
    public readonly Tile.Facing SubDirection;
    public static DigitalCursor Empty => new DigitalCursor(true);

    public DigitalCursor(bool generateAsEmpty = false)
    {
        if (!generateAsEmpty)
        {
            if (CameraManager.Instance != null
                && CameraManager.Instance.mainCamera is Camera camera
                && camera.isActiveAndEnabled
                && Physics.Raycast(
                    camera.ScreenPointToRay(Input.mousePosition), 
                    out RaycastHit hit, 
                    Mathf.Infinity, 
                    GroundMask))
            {
                HitPosition = new Vector2(hit.point.x, hit.point.z);
                Vector2Int roundStep1 = Vector2Int.RoundToInt(HitPosition); //Rounding -.5 to -1, which can cause OOB calls on grid. Intended it to be rounded to 0
                Position = new Vector2Int(Math.Max(roundStep1.x, 0), Math.Max(roundStep1.y, 0)); //This is a really jank solution
                Vector2 delta = HitPosition - Position;
                delta.Normalize();
                SubDirection = Tile.VectorToFacing(delta);
                OnGrid = true;
                return;
            }
        }
        OnGrid = false;
        HitPosition = new Vector2(-1, -1);
        Position = new Vector2Int(-1, -1);
        SubDirection = Tile.Facing.Top;
    }

    /// <summary>
    /// Checks if the cursor is valid to use in current state
    /// </summary>
    /// <param name="cursor"></param>
    /// <returns></returns>
    public bool IsValid() => OnGrid;
}
