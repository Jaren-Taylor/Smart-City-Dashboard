using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button generateMapButton;
    [SerializeField]
    private Button exitButton;

    public string SceneToLoad;

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
        generateMapButton.onClick.AddListener(ShowGenerateMap);
        exitButton.onClick.AddListener(ExitGame);
    }

    public void StartGame()
    {
        Debug.Log("Hello");
        //SceneManager.LoadScene(SceneToLoad);
    }

    public void ShowGenerateMap()
    {
        
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
