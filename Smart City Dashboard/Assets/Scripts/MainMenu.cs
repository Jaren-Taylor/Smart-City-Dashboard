using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject Title;
    [SerializeField]
    private GameObject[] Buttons;

    public void Start()
    {
    }

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
