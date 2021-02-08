using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public static Menu Instance { get; private set; } //  Singleton pattern
    public RectTransform rectTransform;
    public Tab activeTab;
    private float yTo;
    private bool isActive;

    private void Start()
    {
        //Singleton pattern
        if (Instance != null) Destroy(this);
        Instance = this;
        // Deactivate all but the first child tab
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<Tab>().DeActivate();
        }
        activeTab = transform.GetChild(0).gameObject.GetComponent<Tab>();
        //
        yTo = -gameObject.transform.position.y;
        isActive = false;
    }

    private void Update()
    {
        if (transform.position.y != yTo)
        {
            // Move towards destination portions at a time
            Vector3 newPosition = transform.position;
            newPosition.y += (yTo - newPosition.y) / 25;
            transform.position = newPosition;
        }
    }

    public void ToggleMenuHandler()
    {
        float yMove = isActive ? -rectTransform.rect.height : rectTransform.rect.height;
        isActive = !isActive;
        // move the menu offscreen
        yTo += yMove;
    }

    public void ActivateTab(int index)
    {
        transform.GetChild(index).GetComponent<Tab>().Activate();
    }
}
