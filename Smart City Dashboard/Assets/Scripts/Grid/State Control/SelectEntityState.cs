using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEntityState : IGridControlState
{
    private readonly int layerMask = 1 << 7;

    public void OnMouseDown(DigitalCursor location)
    {
        if(location is DigitalCursor //Checks if the Digital Cursor is not null
            && location.OnGrid is true //Checks if the hit location is on the grid
            && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, layerMask) //Checks if the ray hits a target
            && hit.collider.gameObject.TryGetComponent(out Entity entity)) //Checks that the target hit is an entity
        {
            CameraManager.Instance.StartFollowEntity(entity);
        }
    }

    public void OnMouseEnterTile(DigitalCursor location)
    {
    }
      
    public void OnMouseExitTile(DigitalCursor location)
    {
    }

    public void OnPop(DigitalCursor location)
    {
    }

    public void OnPush(DigitalCursor location)
    {

    }
}
