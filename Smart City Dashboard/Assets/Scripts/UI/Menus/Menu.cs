using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RectTransform))]
public class Menu : MonoBehaviour, IWindow, IPointerEnterHandler, IPointerExitHandler
{
    public Key Key;
    public Action<Menu> OnOpen;
    public Action<Menu> OnClose;
    public Action OnEnter;
    public Action OnExit;
    [SerializeField]
    private TabGroup tabGroup;

    protected virtual void Start()
    {
        Close();
        UIManager.Instance.Subscribe(this);
    }

    public virtual void Close()
    {
        OnExit?.Invoke();
        OnClose?.Invoke(this);
        gameObject.SetActive(false);
    }
    public virtual void Open()
    {
        OnOpen?.Invoke(this);
        gameObject.SetActive(true);
    }
    public virtual void Toggle()
    {
        if (IsOpen())
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public virtual bool IsOpen()
    {
        return gameObject.activeSelf;
    }

    public void NextTab()
    {
        if (tabGroup != null)
        {
            tabGroup.NextTab();
        }
    }

    public void OnNumberKeyPress(int value)
    {
        if (tabGroup != null)
        {
            tabGroup.OnNumberKeyPress(value);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke();
    }
}
