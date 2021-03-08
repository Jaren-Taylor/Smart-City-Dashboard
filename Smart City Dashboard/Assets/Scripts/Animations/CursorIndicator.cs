using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorIndicator : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject CursorRoot;
    private GameObject Icon;

    public readonly Vector2Int OffGrid = new Vector2Int(-1, -1);

    public void SetPosition(Vector2Int position)
    {
        if (position != OffGrid) transform.position = new Vector3(position.x, 0f, position.y);
        else transform.position = new Vector3(0, -10, 0);
    }

    public void OnClick() => animator.SetTrigger("OnAction");
    
    public void SetIcon(string iconAddress)
    {
        if (Icon is GameObject) Destroy(Icon);

        if (iconAddress == "") return;

        var iconPrefab = Resources.Load<GameObject>(iconAddress);
        Icon = Instantiate(iconPrefab, CursorRoot.transform);
        Icon.transform.localPosition += Vector3.up * 1.35f;
    }
}
