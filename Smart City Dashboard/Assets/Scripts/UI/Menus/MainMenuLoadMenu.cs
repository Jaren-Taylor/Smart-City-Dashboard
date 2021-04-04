using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLoadMenu : MonoBehaviour
{
    [SerializeField]
    private SaveFileScrollPane scrollPane;
    [SerializeField]
    private MainMenu mainMenu;

    public void OnDisable()
    {
        scrollPane.ClearRegion();
    }

    public void OnEnable()
    {
        scrollPane.PopulateRegion();
    }

    /// <summary>
    /// Edits selected map in the build moade 
    /// </summary>
    public void EditSelectedMap()
    {
        if (ValidateSelection("Please select a file to edit", out string filename))
        {
            SaveGameManager.SetFileName(filename);
            GameSceneManager.LoadScene(SceneIndexes.BUILD, filename);
        }
    }

    /// <summary>
    /// Opens selected map in the dashboard mode
    /// </summary>
    public void OpenSelectedMap()
    {
        if(ValidateSelection("Please select a file to monitor in dashboard", out string filename))
        {
            SaveGameManager.SetFileName(filename);
            GameSceneManager.LoadScene(SceneIndexes.DASHBOARD, filename);
            UIManager.DashboardMode = true;
            //mainMenu.ShowMessagePopup("Build mode not implemented", "Sorry folks, the team hasn't created this feature yet :(");
            //GameSceneManager.LoadScene(SceneIndexes.DASHBOARD, filename);
        }
    }

    /// <summary>
    /// Duplicates selected file and opens prompt to specify new name
    /// </summary>
    public void DuplicateSelectedMap()
    {
        if (ValidateSelection("Please select a file to duplicate", out string filename))
        {
            mainMenu.ShowMessagePopup("Duplicate not implemented", "Sorry folks, the team hasn't created this feature yet :(");
        }
    }

    /// <summary>
    /// Deletes selected file
    /// </summary>
    public void DeleteSelectedMap()
    {
        if (scrollPane.GetSelectedCard() is null)
        {
            mainMenu.ShowMessagePopup("Delete a file", "Please select a file");
        }
        else
        {
            scrollPane.GetSelectedCard().GetCloseButton().onClick.Invoke();
        }
    }

    /// <summary>
    /// Renames selected file
    /// </summary>
    public void RenameSelectedMap()
    {
        if (ValidateSelection("Please select a file to rename", out string filename))
        {
            mainMenu.ShowInputPrompt("Rename File", "Enter new filename:", filename, RenameSubmittedCallback);
        }
    }

    private void RenameSubmittedCallback(bool submitClicked, string enteredText)
    {
        if (submitClicked)
        {
            if(!scrollPane.TryRenameSelection(enteredText, out string errorResponse))
            {
                mainMenu.ShowMessagePopup("Cannot change filename", errorResponse);
            }
        }
    }

    /// <summary>
    /// Validates selected filename, will display errors out to user
    /// </summary>
    /// <param name="noneSelectedMessage"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    private bool ValidateSelection(string noneSelectedMessage, out string filename)
    {
        filename = GetSelectedFilename();
        if (!string.IsNullOrEmpty(filename))
        {
            if (SaveGameManager.FileExists(filename)) return true;
            else mainMenu.ShowMessagePopup("File does not Exist", "Cannot load selected file, it no longer exists.");
        }
        else mainMenu.ShowMessagePopup("No Filename Selected", noneSelectedMessage);
        return false;
    }

    private string GetSelectedFilename() => scrollPane.GetSelectedFile();
}
