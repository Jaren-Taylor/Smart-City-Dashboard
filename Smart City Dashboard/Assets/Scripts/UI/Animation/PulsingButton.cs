using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PulsingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Button attachedButton;

    private float scaleAmount = 1.15f;
    
    private float defaultHeight;
    private float defaultWidth;
    private Vector3 defaultScale;

    private bool resetSelf = false;

    public void Start()
    {
        attachedButton = GetComponent<Button>();

        defaultScale = transform.localScale;
        defaultWidth = transform.RectTransform().rect.width;
        defaultHeight = transform.RectTransform().rect.height;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(attachedButton.interactable) LeanTween.scale(gameObject, CalculateEvenHeightBasedScale(scaleAmount + .1f), .1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (attachedButton.interactable) LeanTween.scale(gameObject, CalculateEvenHeightBasedScale(scaleAmount), 1.091f).setLoopPingPong();
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
        if (attachedButton.interactable)
        {
            LeanTween.cancel(gameObject);
            transform.localScale = defaultScale;
        }
    }

    void OnDisable()
    {
        LeanTween.cancel(gameObject);
        resetSelf = true;
    }

    void OnEnable()
    {
        if (resetSelf)
        {
            transform.localScale = defaultScale;
            resetSelf = false;
        }
    }
}
