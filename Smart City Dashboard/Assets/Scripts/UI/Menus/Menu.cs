using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class Menu : MonoBehaviour, IWindow, IPointerEnterHandler, IPointerExitHandler
{
    public UIManager uiManager;
    public TabGroup TabGroup;

    private void Start()
    {
        Close();
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual bool IsOpen()
    {
        return gameObject.activeSelf;
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Toggle()
    {
        if (IsOpen())
            Close();
        else
            Open();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        uiManager.OnEnteringUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiManager.OnExitingUI();
    }
}
