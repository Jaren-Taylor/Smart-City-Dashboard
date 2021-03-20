using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    #if UNITY_EDITOR

    [MenuItem("GameObject/UI/Linear Progress Bar")]
    public static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Linear Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }

    #endif


    public int minimum;
    public int maximum;
    public int current;

    public Image mask;

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();   
    }

    void GetCurrentFill()
    {
        if (maximum <= 0 || mask == null || minimum < 0) return;

        float currOffset = current - minimum;
        float maxOffset = maximum - minimum;

        float fillAmount = currOffset / maxOffset;
        mask.fillAmount = fillAmount;
    }
}
