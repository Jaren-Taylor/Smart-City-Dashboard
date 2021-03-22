using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntDisplayableSlider : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI label;

    public void UpdateSliderText(float value) => UpdateSliderText(Mathf.RoundToInt(value));
    public void UpdateSliderText(int value) => label.SetText(value.ToString());
}
