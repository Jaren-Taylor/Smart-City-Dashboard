using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Data container to hold runtime configured settings
/// </summary>
public static class Config
{
    public static float boundaryFraction = .2f; //What fraction of the screen border is used to pan view
    public static int panSpeed = 1; //Default speed for panning
    public static float panZoomSenstivity = 4f; //How much the zoom affects the pan. (When zoomed in, pan scales less)
    public static float zoomScale = .05f; //How fast the scroll wheel zooms in


    public const float smoothTime = 0.3F; //Used for physics translations.

    public const float minSize = 1f; // Most zoomed the camera can get
    public const float maxSize = 25f; // How far zoomed out the camera can
    public const float defaultSize = 5f; // Default zoom
}
