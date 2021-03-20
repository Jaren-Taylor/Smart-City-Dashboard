using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    private SceneIndexes currentScene;
    private List<AsyncOperation> runningOperations;
    private float totalSceneProgress;

    [SerializeField]
    private GameObject loadingScreen;
    [SerializeField]
    private ProgressBar progressBar;
    [SerializeField]
    private TextMeshProUGUI loadingScreenTitle;
    [SerializeField]
    private Image dotDotDotDotDotDotMask;

    private int dotCount = 0;

    private WaitForSeconds wfsObj = new WaitForSeconds(.2f);

    private void Awake()
    {
        if (Instance != null) throw new System.Exception("There can only be one instance of the game scene manager");

        Instance = this;

        runningOperations = new List<AsyncOperation>();
        SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE, LoadSceneMode.Additive);
        currentScene = SceneIndexes.TITLE;

        loadingScreen.gameObject.SetActive(false);
        dotDotDotDotDotDotMask.fillAmount = 0f;

    }

    public static void LoadScene(SceneIndexes newScene, string loadingMessage = "")
    {
        if (Instance == null) throw new System.Exception("Mike added this: start the game from the LOADING SCENE to be able to change scenes.");
        Instance.InternalLoadScene(newScene, GetLoadMessage(newScene, loadingMessage));
    }

    private static string GetLoadMessage(SceneIndexes newScene, string loadingMessage) => (newScene, loadingMessage) switch
    {
        (SceneIndexes.BUILD, "") => "Loading Empty City",
        (SceneIndexes.BUILD, _) => "Loading: " + loadingMessage,
        (SceneIndexes.TITLE, _) => "Loading Main Menu",
        _ => "Welcome..."
    };


    private void InternalLoadScene(SceneIndexes newScene, string loadingMessage)
    {
        if (newScene == SceneIndexes.LOADING) throw new System.Exception("Can't load the load scene dummy, it loads itself.");
        if (runningOperations.Count != 0) throw new System.Exception("Cannont change screens while already loading");

        loadingScreenTitle.text = loadingMessage;
        loadingScreen.gameObject.SetActive(true);

        runningOperations.Add(SceneManager.UnloadSceneAsync((int)currentScene));
        runningOperations.Add(SceneManager.LoadSceneAsync((int)newScene, LoadSceneMode.Additive));

        currentScene = newScene;

        StartCoroutine(IncrementDotDotDotDotDotDot());
        StartCoroutine(GetSceneLoadProgress());
    }

    private IEnumerator GetSceneLoadProgress()
    {
        for(int i = 0; i < runningOperations.Count; i++)
        {
            while (!runningOperations[i].isDone)
            {
                totalSceneProgress = 0;

                foreach(AsyncOperation operation in runningOperations)
                {
                    totalSceneProgress += operation.progress;
                }

                totalSceneProgress = (totalSceneProgress / runningOperations.Count) * 100f;

                progressBar.current = Mathf.RoundToInt(totalSceneProgress);

                yield return null;
            }
        }

        runningOperations.Clear();
        dotDotDotDotDotDotMask.fillAmount = 0f;
        loadingScreen.gameObject.SetActive(false);
    }

   

    private IEnumerator IncrementDotDotDotDotDotDot()
    {
        while (loadingScreen.gameObject.activeInHierarchy)
        {
            dotCount++;
            if (dotCount > 5)
            {
                if (dotCount >= 8)
                {
                    dotCount = 0;
                    dotDotDotDotDotDotMask.fillAmount = 0f;
                }
            }
            else dotDotDotDotDotDotMask.fillAmount = dotCount / 5f;

            yield return new WaitForSeconds(.2f);
        }
    }
}

public enum SceneIndexes
{
    LOADING = 0,
    TITLE = 1,   
    BUILD = 2,
    DASHBOARD = 3
}
