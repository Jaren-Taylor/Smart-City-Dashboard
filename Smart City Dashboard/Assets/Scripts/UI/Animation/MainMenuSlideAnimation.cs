using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuSlideAnimation : MonoBehaviour
{
    private const int maxSlideSpeedModifier = 10;

    [SerializeField]
    private GameObject MenuContent;
    [SerializeField]
    private GameObject Title;
    [SerializeField]
    private Button[] Buttons;

    [SerializeField]
    [Range(1, maxSlideSpeedModifier)]
    private int slideSpeedModifier;

    [SerializeField]
    [Range(1, maxSlideSpeedModifier)]
    private int buttonSlideSpeedModifier;

    private float[] buttonTargets;
    private float buttonSlideSpeed;

    int buttonArrivedCounter = 0;

    public void Start()
    {
        buttonTargets = new float[Buttons.Length];
        for(int i = 0; i < Buttons.Length; i++)
        {
            buttonTargets[i] = Buttons[i].gameObject.transform.localPosition.y;
            Vector2 localOffset = Title.transform.localPosition;
            localOffset.y -= Title.RectTransform().rect.height / 2f;
            localOffset.y += Buttons[i].gameObject.RectTransform().rect.height / 2f;

            Buttons[i].transform.localPosition = localOffset;
            Buttons[i].interactable = false;
        }

        Vector3 offset = new Vector3(0, Screen.height / 2f + MenuContent.RectTransform().rect.height / 2);

        float delta = Mathf.Abs(MenuContent.transform.localPosition.y - offset.y);
        float slideInTime = 0.1f * (maxSlideSpeedModifier - slideSpeedModifier);
        buttonSlideSpeed = (delta) * (.2f * (buttonSlideSpeedModifier));
        TweenIn(MenuContent, slideInTime, offset).setOnComplete(OnTitleArrived).setEase(LeanTweenType.linear);
    }

    private LTDescr TweenIn(GameObject obj, float delta, Vector3 offscreenPoint)
    {
        Vector3 target = obj.transform.localPosition;
        obj.transform.localPosition = offscreenPoint;
        return LeanTween.moveLocal(obj, target, delta);
    }

    private void OnTitleArrived()
    {
        for(int i = 0; i < Buttons.Length; i++)
        {
            float timing = Mathf.Abs(buttonTargets[i] - Buttons[i].transform.localPosition.y) / buttonSlideSpeed;
            LeanTween.moveLocalY(Buttons[i].gameObject, buttonTargets[i], timing).setEase(LeanTweenType.linear).setOnComplete(OnButtonArrived);
        }
    }

    private void OnButtonArrived()
    {
        Buttons[buttonArrivedCounter++].interactable = true;  
    }
}
