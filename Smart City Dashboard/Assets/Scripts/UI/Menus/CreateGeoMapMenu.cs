using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreateGeoMapMenu : MonoBehaviour
{
    [SerializeField]
    private bool debugMode;

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
    [SerializeField]
    private GameObject submissionFields;

    private bool IsQuerying = false;

    private int previewMapSize = -1;

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

            if(string.IsNullOrEmpty(rawLocationName) || string.IsNullOrWhiteSpace(rawLocationName))
            {
                mainMenu.ShowMessagePopup("Location Invalid", "Please enter a location to search for.");
                return;
            }

            int queryMapSize = Mathf.RoundToInt(rawMapSize * 4.6f + 40);

            if(GoogleMapsTestQuery.TryCreateQuery(queryMapSize, zoomLevel, rawLocationName, out string url)  )
            {
                previewMapSize = rawMapSize;
                //QueryImageCallback(testImage);
                IsQuerying = true;
                StartCoroutine(GoogleMapsTestQuery.GetTexture(url, QueryImageCallback));

            }
            else mainMenu.ShowMessagePopup("Query Failed", "The query could not be completed at this time.");
        }
        else mainMenu.ShowMessagePopup("No stored API Key", "Please enter a Google Maps API Key in the settings page.");
    }

    public void TryCreateCity()
    {
        string filename = filenameField.text.Trim();

        if (SaveGameManager.FileNameInvalidOrTaken(filename, out string response))
        {
            mainMenu.ShowMessagePopup("Invalid Filename", response);
            return;
        }

        if (placeholderMask.activeInHierarchy)
        {
            mainMenu.ShowMessagePopup("No Map Found", "Please preview map before creation.");
            return;
        }

        Texture2D rescaled = previewImage.sprite.texture.CopyAndRescale(previewMapSize, previewMapSize);

        var (tileMap, texture) = GeoDataExtractor.ExtractTileMap(rescaled);

        if (debugMode)
        {
            SetPreviewImage(texture);
            return;
        }


        if(tileMap == null)
        {
            mainMenu.ShowMessagePopup("Extraction Failed", "Could not extract a city from the location.");
            return;
        }

        SaveGameManager.SetFileName(filename);
        if (SaveGameManager.WriteMapToFile(filename, tileMap))
        {
            GameSceneManager.LoadScene(SceneIndexes.BUILD, filename);
        }
        else
        {
            mainMenu.ShowMessagePopup("Couldn't Write to File", "Could not write to the specified filename.");
            return;
        }
    }

    public void ClearPreviewImage() => ClearPreviewImage("No Image");
    
    public void ClearPreviewImage(string message)
    {
        SetPreviewImage(null);
        previewStatusMessage.SetText(message);
        submissionFields.SetActive(false);
    }

    private void SetPreviewImage(Texture2D texture)
    {
        if (texture is null) 
        {
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
        if(texture is null || texture.width <= 40 || texture.height <= 40)
        {
            ClearPreviewImage("Location Query Failed");
        }
        else
        {
            submissionFields.SetActive(true);
            Texture2D trimmed = texture.CopyAndTrimToSize(texture.width - 40, texture.height - 40);
            SetPreviewImage(trimmed);
        }
    }
}
