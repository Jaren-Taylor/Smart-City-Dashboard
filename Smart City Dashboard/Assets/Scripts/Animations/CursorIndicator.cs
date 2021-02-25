using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorIndicator : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public readonly Vector2Int OffGrid = new Vector2Int(-1, -1);

    public void SetPosition(Vector2Int position)
    {
        if (position != OffGrid) transform.position = new Vector3(position.x, 0f, position.y);
        else transform.position = new Vector3(0, -10, 0);
    }

    public void OnClick() => animator.SetTrigger("OnAction");
    
}
