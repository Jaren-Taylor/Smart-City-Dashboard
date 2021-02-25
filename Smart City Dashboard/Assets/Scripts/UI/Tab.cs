using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    private Menu parent;
    private List<Button> buttons = new List<Button>();

    private void Start()
    {
        parent = transform.parent.GetComponent<Menu>();
        if (parent == null) throw new System.Exception("Parent must have a Menu script!");
        InitializeButtonList();
    }

    /// <summary>
    /// Reads through every transform child and adds any objects with a button component to the buttons list
    /// </summary>
    private void InitializeButtonList()
    {
        buttons.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            TryFetchButton(i);
        }
    }

    /// <summary>
    /// Attempts to fetch the button class on a given transform child index
    /// </summary>
    /// <param name="i">The transform.getChild index</param>
    private void TryFetchButton(int i)
    {
        if (transform.GetChild(i).TryGetComponent(out Button button))
        {
            buttons.Add(button);
        }
    }

    /// <summary>
    /// Invokes the ith buttons onClick event
    /// </summary>
    /// <param name="index">The index of the button to call</param>
    public void ButtonClick(int i)
    {
        if (i < buttons.Count) buttons[i].onClick.Invoke();
    }

    /// <summary>
    /// Deactivates each transform child and sets tab transparency to 0.5f
    /// </summary>
    public void DeActivate()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            // make transparent
            SetTransparency(0.5f);
        }
    }

    /// <summary>
    /// Activates each transform child and sets tab transparency to 1
    /// </summary>
    public void Activate()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            // make fully visible
            SetTransparency(1);
        }
    }

    /// <summary>
    /// Sets the transparency of the tab's image
    /// </summary>
    /// <param name="a"></param>
    private void SetTransparency(float a)
    {
        Image image = GetComponent<Image>();
        Color color = image.color;
        color.a = a;
        image.color = color;
    }
}
