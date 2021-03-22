using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreateGeoMapMenu : MonoBehaviour
{
    [SerializeField]
    private MainMenu mainMenu;
    [SerializeField]
    private TextMeshProUGUI previewStatusMessage;
    [SerializeField]
    private Image previewImage;
    [SerializeField]
    private TMP_InputField locationField;
    [SerializeField]
    private Slider zoomLevel;
    [SerializeField]
    private Slider mapSize;
    [SerializeField]
    private TMP_InputField filenameField;


    private (string rawLocationName, int zoomLevel, int rawMapSize) GetRawQueryInputs() =>
        (locationField.text, (int)zoomLevel.value, (int)mapSize.value);
    public void TryShowMapPreview()
    {
       // GoogleMapsTestQuery.
    }

    public void TryCreateCity()
    {
        string text = filenameField.text.Trim();

        if (SaveGameManager.FileNameInvalidOrTaken(text, out string response))
        {
            mainMenu.ShowPopup("Invalid Filename", response);
            return;
        }
    }
}
