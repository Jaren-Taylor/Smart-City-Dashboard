using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject createMenu;
    [SerializeField]
    private GameObject loadMenu;
    [SerializeField]
    private GameObject settingsMenu;
    [SerializeField]
    private GameObject aboutUsMenu;
    [SerializeField]
    private PopupMenu notificationPopup;

    public void ShowMessagePopup(string title, string body)
    {
        notificationPopup.ShowMessageBox(title, body);
        notificationPopup.gameObject.SetActive(true);
    }

    public void ShowInputPrompt(string title, string inputRequest, string defaultText, Action<bool, string> cancelSubmitCallback)
    {
        notificationPopup.ShowInputField(title, inputRequest, defaultText, cancelSubmitCallback);
        notificationPopup.gameObject.SetActive(true);
    }

    public void NavigateToMenu(GameObject menu)
    {
        DisableAllMenus();
        EnableIfNotActive(menu);
    }

    public void LoadFileInBuild()
    {
        GameSceneManager.LoadScene(SceneIndexes.BUILD);
    }

    public void LoadFileInDashboard()
    {
        GameSceneManager.LoadScene(SceneIndexes.DASHBOARD);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void DisableAllMenus()
    {
        DisableIfActive(mainMenu);
        DisableIfActive(createMenu);
        DisableIfActive(loadMenu);
        DisableIfActive(settingsMenu);
        DisableIfActive(aboutUsMenu);
    }

    private void DisableIfActive(GameObject go)
    {
        if (go.activeInHierarchy) go.SetActive(false);
    }

    private void EnableIfNotActive(GameObject go)
    {
        if (go.activeInHierarchy is false) go.SetActive(true);
    }
}
