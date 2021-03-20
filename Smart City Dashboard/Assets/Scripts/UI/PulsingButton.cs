using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PulsingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private float scaleAmount = 1.15f;
    
    private float defaultHeight;
    private float defaultWidth;
    private Vector3 defaultScale;

    public void OnPointerClick(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, CalculateEvenHeightBasedScale(scaleAmount + .1f), .1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, CalculateEvenHeightBasedScale(scaleAmount), 1f).setLoopPingPong();
    }

    private Vector3 CalculateEvenHeightBasedScale(float heightScale)
    {
        float widthScale = ((heightScale - 1) * defaultHeight) / defaultWidth;
        widthScale += 1;

        return new Vector3(widthScale, heightScale);
    }

    private Vector3 CalculateEvenWidthBasedScale(float widthScale)
    {
        float heightScale = ((widthScale - 1) * defaultWidth) / defaultHeight;
        heightScale += 1;

        return new Vector3(widthScale, heightScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        transform.localScale = defaultScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultScale = transform.localScale;
        defaultWidth = transform.RectTransform().rect.width;
        defaultHeight = transform.RectTransform().rect.height;
    }
}
