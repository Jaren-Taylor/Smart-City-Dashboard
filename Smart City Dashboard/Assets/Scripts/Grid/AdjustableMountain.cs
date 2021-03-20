using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustableMountain : MonoBehaviour
{
    [SerializeField]
    private GameObject mountainModel;

    public void SetHeightScale(float scale)
    {
        mountainModel.transform.localScale = new Vector3(mountainModel.transform.localScale.x, mountainModel.transform.localScale.y, scale / 2f);
    }

}
