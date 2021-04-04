using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour
{
    [SerializeField] private InputBox inputField;
    [SerializeField] private MessageBox messageBox;

    public void ShowMessageBox(string title, string message)
    {
        messageBox.SetValues(title, message);
        ShowOnly(messageBox.gameObject);
    }

    public void ShowInputField(string title, string fieldLabel, string defaultText, Action<bool, string> closeSubmitFunction)
    {
        SetInputFieldValues(title, fieldLabel, defaultText, closeSubmitFunction);
        ShowOnly(inputField.gameObject);
    }

    private void SetInputFieldValues(string title, string fieldLabel, string defaultText, Action<bool, string> closeSubmitFunction)
    {
        inputField.Title.text = title;
        inputField.Body.text = fieldLabel;
        inputField.Input.text = defaultText;
        inputField.Callback = closeSubmitFunction;
    }
    
    private void ShowOnly(GameObject popup)
    {
        if (popup != messageBox.gameObject) messageBox.gameObject.SetActive(false);
        if (popup != inputField.gameObject) inputField.gameObject.SetActive(false);
        popup.SetActive(true);
    }

    public void TFSubmitClicked()
    {
        inputField.Callback?.Invoke(true, inputField.Input.text);
        inputField.Callback = null;
    }

    public void TFCloseClicked()
    {
        inputField.Callback?.Invoke(false, "");
        inputField.Callback = null;
    }
}
