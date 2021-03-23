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
    private GameObject placeholderMask;
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
    private CoroutineWithData couroutine;
    private bool IsQuerying = false;

    private (string rawLocationName, int zoomLevel, int rawMapSize) GetRawQueryInputs() =>
        (locationField.text, (int)zoomLevel.value, (int)mapSize.value);
    public void TryShowMapPreview()
    {
        if (IsQuerying) 
        {
            mainMenu.ShowMessagePopup("Already in progress", "Already retrieving image"); 
            return; 
        }

        if (APIKey.HasKey())
        {
            var (rawLocationName, zoomLevel, rawMapSize) = GetRawQueryInputs();
            int queryMapSize = Mathf.RoundToInt(rawMapSize * 4.6f + 40);

            if(GoogleMapsTestQuery.TryCreateQuery(queryMapSize, zoomLevel, rawLocationName, out string url)  )
            {
                IsQuerying = true;
                StartCoroutine(GoogleMapsTestQuery.GetTexture(url, QueryImageCallback));

            }
            else mainMenu.ShowMessagePopup("Query Failed", "The query could not be completed at this time.");
        }
        else mainMenu.ShowMessagePopup("No stored API Key", "Please enter a Google Maps API Key in the settings page.");
    }

    public void TryCreateCity()
    {
        string text = filenameField.text.Trim();

        if (SaveGameManager.FileNameInvalidOrTaken(text, out string response))
        {
            mainMenu.ShowMessagePopup("Invalid Filename", response);
            return;
        }
    }

    private void SetPreviewImage(Texture2D texture)
    {
        if (texture is null) {
            placeholderMask.SetActive(true);
        }
        else
        {
            placeholderMask.SetActive(false);
            this.previewImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        
    }

    private void QueryImageCallback(Texture2D texture)
    {
        IsQuerying = false;
        if(texture is null)
        {
            SetPreviewImage(null);
            previewStatusMessage.SetText("Image Query Failed");
        }
        else
        {
            SetPreviewImage(texture);
        }


    }

   
}
