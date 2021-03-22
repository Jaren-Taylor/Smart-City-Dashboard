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
        if (APIKey.HasKey())
        {
            var (rawLocationName, zoomLevel, rawMapSize) = GetRawQueryInputs();
            int querySize = Mathf.RoundToInt(rawMapSize * 4.6f + 40);

            if(GoogleMapsTestQuery.MakeQuery(querySize, zoomLevel, rawLocationName))
            {

            }
            else mainMenu.ShowPopup("Query Failed", "The query could not be completed at this time.");
        }
        else mainMenu.ShowPopup("No stored API Key", "Please enter a Google Maps API Key in the settings page.");
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
