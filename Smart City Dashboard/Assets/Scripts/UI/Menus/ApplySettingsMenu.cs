using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ApplySettingsMenu : MonoBehaviour
{
    [SerializeField]
    private MainMenu mainMenu;
    [SerializeField]
    private TMP_InputField apiKeyField; 

    public void TrySaveAPIKey()
    {
        if (APIKey.IsKeyValid(apiKeyField.text))
        {
            if (APIKey.TrySaveAPIKey(apiKeyField.text))
            {
                mainMenu.ShowPopup("Success!", "The API Key was saved successfuly!");
            }
            else mainMenu.ShowPopup("IO Error", "Unable to write the Key to disk. It will be stored for this session only.");
        }
        else mainMenu.ShowPopup("Invalid API Key", "The API Key entered was invalid.");
    }
}
