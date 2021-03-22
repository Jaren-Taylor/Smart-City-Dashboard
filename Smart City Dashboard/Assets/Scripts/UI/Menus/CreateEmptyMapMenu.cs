using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreateEmptyMapMenu : MonoBehaviour
{
    [SerializeField]
    private MainMenu mainMenu;

    [SerializeField]
    private TextMeshProUGUI citySize;
    
    [SerializeField]
    private TMP_InputField cityName;

    public void UpdateSliderText(float value) => UpdateSliderText(Mathf.RoundToInt(value));
    public void UpdateSliderText(int value) => citySize.SetText(value.ToString());


    public void TryCreateCity()
    {
        string fileName = cityName.text.Trim();

        if (int.TryParse(citySize.text, out int size))
        {
            if(SaveGameManager.FileNameInvalidOrTaken(fileName, out string response))
            {
                mainMenu.ShowPopup("Invalid Filename", response);
            }
            else //Input validated
            {
                var tileGrid = new TileGrid(size, size);

                SaveGameManager.SetFileName(fileName);
                SaveGameManager.WriteMapToFile(tileGrid);

                GameSceneManager.LoadScene(SceneIndexes.BUILD, fileName);
            }
        }
    }
}
