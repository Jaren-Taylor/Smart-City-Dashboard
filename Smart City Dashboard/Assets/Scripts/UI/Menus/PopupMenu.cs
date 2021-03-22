using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject messageBox;
    [SerializeField]
    private GameObject inputField;

    [SerializeField]
    private TextMeshProUGUI mbTitle;
    [SerializeField]
    private TextMeshProUGUI mbMessage;

    [SerializeField]
    private TextMeshProUGUI tfTitle;
    [SerializeField]
    private TextMeshProUGUI tfFieldLabel;
    [SerializeField]
    private TMP_InputField tfInput;

    private Action<bool, string> tfCallback;

    public void ShowMessageBox(string title, string message)
    {
        SetMessageBoxValues(title, message);
        ShowOnly(messageBox);
    }

    public void ShowInputField(string title, string fieldLabel, string defaultText, Action<bool, string> closeSubmitFunction)
    {
        SetInputFieldValues(title, fieldLabel, defaultText, closeSubmitFunction);
        ShowOnly(inputField);
    }

    private void SetMessageBoxValues(string title, string message)
    {
        this.mbTitle.SetText(title);
        this.mbMessage.SetText(message);
    }

    private void SetInputFieldValues(string title, string fieldLabel, string defaultText, Action<bool, string> closeSubmitFunction)
    {
        tfTitle.SetText(title);
        tfFieldLabel.SetText(fieldLabel);
        tfInput.text = defaultText;
        tfCallback = closeSubmitFunction;
    }
    
    private void ShowOnly(GameObject popup)
    {
        if (popup != messageBox) messageBox.SetActive(false);
        if (popup != inputField) inputField.SetActive(false);
        popup.SetActive(true);
    }

    public void TFSubmitClicked()
    {
        tfCallback?.Invoke(true, tfInput.text);
        tfCallback = null;
    }

    public void TFCloseClicked()
    {
        tfCallback?.Invoke(false, "");
        tfCallback = null;
    }
}
