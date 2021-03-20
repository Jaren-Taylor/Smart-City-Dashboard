using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        GameSceneManager.LoadScene(SceneIndexes.BUILD);
    }

    public void ShowGenerateMap()
    {
        Debug.Log("Add a scene here");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
