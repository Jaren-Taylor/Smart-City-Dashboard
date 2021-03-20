using System.Collections.Generic;
using UnityEngine;

public class UIElementManager : MonoBehaviour
{
    public static float Margin = 8;
    public static float halfMargin = Margin / 2;
    public static float GlideSpeed = 25;
 
    public List<UIClickable> elements = new List<UIClickable>();
    private Dictionary<UIClickable, RectTransform> rectTransforms = new Dictionary<UIClickable, RectTransform>();
    private Dictionary<UIClickable, Vector3> destinations = new Dictionary<UIClickable, Vector3>();
    private Vector3 positionCursor = new Vector3(Margin, -Margin, 0);

    private void Update()
    {
        UpdatePositions();
    }

    /// <summary>
    /// Updates each GameObjects position on the screen
    /// </summary>
    private void UpdatePositions()
    {
        foreach (UIClickable element in elements)
        {
            // only update position if it needs it
            if (rectTransforms[element].localPosition != destinations[element]) {
                rectTransforms[element].localPosition += GlideAmount(destinations[element], rectTransforms[element].localPosition);
            }
        }
    }

    /// <summary>
    /// Calculates the distance between the destination and current position, then divides by the glideSpeed
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    private Vector3 GlideAmount(Vector3 destination, Vector3 localPosition) => new Vector3((destination.x - localPosition.x) / GlideSpeed, (destination.y - localPosition.y) / GlideSpeed, 0);

    /// <summary>
    /// Add a GameObject to the manager's system. GameObject is assumed to contain a RectTransform
    /// </summary>
    /// <param name="element"></param>
    public void Add(UIClickable element)
    {
        var rectTransform = element.GetComponent<RectTransform>();
        //element.OnDestroyElement += Remove;
        elements.Add(element);
        AddToDictionaries(element, rectTransform);
        positionCursor -= new Vector3 (0, rectTransform.rect.height + Margin, 0);
    }

    /// <summary>
    /// Adds the GameObject to the RectTransform and destination dictonaries. the positionCursor is used for its destination
    /// </summary>
    /// <param name="element"></param>
    /// <param name="rectTransform"></param>
    private void AddToDictionaries(UIClickable element, RectTransform rectTransform)
    {
        rectTransforms.Add(element, rectTransform);
        destinations.Add(element, new Vector3(positionCursor.x, positionCursor.y, positionCursor.z));
    }

    /// <summary>
    /// Removes a GameObject from the manager's system
    /// </summary>
    /// <param name="element"></param>
    public void Remove(UIClickable element)
    {
        UpdateDictionaryPositions(element);
        RemoveFromDictionaries(element);
        elements.Remove(element);
    }

    /// <summary>
    /// Removes a GameObject from each dictionary the manager uses
    /// </summary>
    /// <param name="element"></param>
    private void RemoveFromDictionaries(UIClickable element)
    {
        rectTransforms.Remove(element);
        destinations.Remove(element);
    }

    /// <summary>
    /// Only used for when a GameObject is removed from the manager, and the dictionaries need to be updated
    /// </summary>
    /// <param name="elementRemoved"></param>
    private void UpdateDictionaryPositions(UIClickable elementRemoved)
    {
        int indexRemoved = elements.IndexOf(elementRemoved);
        Vector3 spaceCreated;
        if (indexRemoved < elements.Count - 1)
        {
            spaceCreated = rectTransforms[elementRemoved].localPosition - rectTransforms[elements[indexRemoved + 1]].localPosition;
        } else
        {
            spaceCreated = rectTransforms[elementRemoved].localPosition - positionCursor;
        }
        positionCursor += spaceCreated;
        // We need to update the other elements if the element removed was not the last element
        if (indexRemoved < elements.Count - 1)
        {
            for (int i = indexRemoved+1; i < elements.Count; i++)
            {
                destinations[elements[i]] += spaceCreated;
            }
        }
    }
}
