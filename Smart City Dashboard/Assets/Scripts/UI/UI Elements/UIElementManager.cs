using System.Collections.Generic;
using UnityEngine;

public class UIElementManager : MonoBehaviour
{
    public static float Margin = 8;
    public static float halfMargin = Margin / 2;
    public static float GlideSpeed = 25;
 
    public List<GameObject> elements = new List<GameObject>();
    private Dictionary<GameObject, RectTransform> rectTransforms = new Dictionary<GameObject, RectTransform>();
    private Dictionary<GameObject, Vector3> destinations = new Dictionary<GameObject, Vector3>();
    private Vector3 positionCursor = new Vector3(Margin, -Margin, 0);

    private void Update()
    {
        UpdatePositions();
    }

    private void UpdatePositions()
    {
        foreach (GameObject element in elements)
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

    public void Add(GameObject element)
    {
        var rectTransform = element.GetComponent<RectTransform>();
        elements.Add(element);
        AddToDictionaries(element, rectTransform);
        positionCursor -= new Vector3 (0, rectTransform.rect.height + Margin, 0);
    }

    private void AddToDictionaries(GameObject element, RectTransform rectTransform)
    {
        rectTransforms.Add(element, rectTransform);
        destinations.Add(element, new Vector3(positionCursor.x, positionCursor.y, positionCursor.z));
    }

    public void Remove(GameObject element)
    {
        UpdateDictionaryPositions(element);
        RemoveFromDictionaries(element);
        elements.Remove(element);
    }

    private void ResetPositionCursor() => positionCursor = new Vector3(Margin, -Margin, 0);

    private void RemoveFromDictionaries(GameObject element)
    {
        rectTransforms.Remove(element);
        destinations.Remove(element);
    }

    private void UpdateDictionaryPositions(GameObject elementRemoved)
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
