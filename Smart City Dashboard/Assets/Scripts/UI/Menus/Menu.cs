using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour, IFocusableWindow
{
    [HideInInspector]
    protected RectTransform menuBounds;
    public EUIPosition uiPosition = EUIPosition.Bottom;
    protected float destination;
    protected int glideSpeed = 25;

    private bool isOnScreen;

    protected void Start()
    {
        // Assumed to be used in child classes for use in movement calculations
        menuBounds = gameObject.GetComponent<RectTransform>();
        InitializeGlideAmount();
        JumpToDestination();
        isOnScreen = false;
    }

    /// <summary>
    /// If needed, glides the menu into/out of place
    /// </summary>
    private void Update()
    {
        // dont try to move if we're at our target position
        switch (uiPosition)
        {
            case EUIPosition.Top:
            case EUIPosition.Bottom:
                if (transform.position.y != destination) GlideTowardsDestination();
                break;
            case EUIPosition.Left:
            case EUIPosition.Right:
                if (transform.position.x != destination) GlideTowardsDestination();
                break;
        }
    }

    /// <summary>
    /// Set the Menu's glide amount based on its EUIPosition
    /// </summary>
    private void InitializeGlideAmount()
    {
        switch (uiPosition)
        {
            case EUIPosition.Top:
                destination = +menuBounds.rect.height;
                break;
            case EUIPosition.Bottom:
                destination = -menuBounds.rect.height;
                break;
            case EUIPosition.Left:
                destination = -menuBounds.rect.width;
                break;
            case EUIPosition.Right:
                destination = +menuBounds.rect.width;
                break;
        }
    }

    /// <summary>
    /// Glide the menu in and out of view
    /// </summary>
    protected virtual void GlideTowardsDestination()
    {
        // Move towards destination portions at a time
        Vector3 newPosition = transform.position;
        switch (uiPosition)
        {
            case EUIPosition.Top:
            case EUIPosition.Bottom:
                newPosition.y += YGlideAmount(newPosition.y);
                break;
            case EUIPosition.Left:
            case EUIPosition.Right:
                newPosition.x += XGlideAmount(newPosition.x);
                break;
        }
        transform.position = newPosition;
    }

    /// <summary>
    /// Calculates the distance between the destination and current position, then divides by the glideSpeed
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    private float YGlideAmount(float y) => (destination - y) / glideSpeed;

    /// <summary>
    /// Calculates the distance between the destination and current position, then divides by the glideSpeed
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private float XGlideAmount(float x) => (destination - x) / glideSpeed;

    /// <summary>
    /// Quickly move the menu in and out of view
    /// </summary>
    private void JumpToDestination()
    {
        // Move towards destination portions at a time
        Vector3 newPosition = transform.position;
        switch (uiPosition)
        {
            case EUIPosition.Top:
                newPosition.y += destination - newPosition.y;
                break;
            case EUIPosition.Bottom:
                newPosition.y += destination - newPosition.y;
                break;
            case EUIPosition.Left:
                newPosition.x += destination - newPosition.x;
                break;
            case EUIPosition.Right:
                newPosition.x += destination - newPosition.x;
                break;
        }
        transform.position = newPosition;
    }

    /// <summary>
    /// Opens and closes the menu
    /// </summary>
    public virtual void ToggleMenuHandler()
    {
        float movementDelta = 0;
        // move with respect to the menu's RectTransform width or height
        switch (uiPosition)
        {
            case EUIPosition.Top:
            case EUIPosition.Bottom:
                movementDelta = isOnScreen ? -menuBounds.rect.height : menuBounds.rect.height;
                break;
            case EUIPosition.Left:
            case EUIPosition.Right:
                movementDelta = isOnScreen ? -menuBounds.rect.width : menuBounds.rect.width;
                break;

        }
        isOnScreen = !isOnScreen;
        // move the menu offscreen
        destination += movementDelta;
    }

    /// <summary>
    /// Returns true if window is fully visible on screen
    /// </summary>
    public bool IsFullyVisible()
    {
        return isOnScreen;
    }

    public void OnNumberKeyPress(int value)
    {
        return;
    }
}