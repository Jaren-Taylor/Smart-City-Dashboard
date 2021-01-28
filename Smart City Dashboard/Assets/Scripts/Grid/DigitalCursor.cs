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
    public readonly static int GroundMask = 1 << LayerMask.NameToLayer("Grid");

    public readonly bool OnGrid = false;
    public readonly Vector2 HitPosition = new Vector2(-1,-1);
    public readonly Vector2Int Position = new Vector2Int(-1, -1);
    public readonly Tile.Facing SubDirection = Tile.Facing.Top;

    public DigitalCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GroundMask))
        {
            HitPosition = new Vector2(hit.point.x, hit.point.z);
            Vector2Int roundStep1 = Vector2Int.RoundToInt(HitPosition); //Rounding -.5 to -1, which can cause OOB calls on grid. Intended it to be rounded to 0
            Position = new Vector2Int(Math.Max(roundStep1.x, 0), Math.Max(roundStep1.y, 0)); //This is a really jank solution
            SubDirection = CalculateSubDirection();
            OnGrid = true;
        }
    }

    private Tile.Facing CalculateSubDirection()
    {
        Vector2 delta = HitPosition - Position;
        delta.Normalize();
        return Tile.VectorToFacing(delta);
    }
}
