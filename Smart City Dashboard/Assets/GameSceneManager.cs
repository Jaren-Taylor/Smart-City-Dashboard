using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    private void Awake()
    {
        if (Instance != null) throw new System.Exception("There can only be one instance of the game scene manager");

        Instance = this;

        runningOperations = new List<AsyncOperation>();
        SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE, LoadSceneMode.Additive);
        currentScene = SceneIndexes.TITLE;

        loadingScreen.gameObject.SetActive(false);
    }

    public static void LoadScene(SceneIndexes newScene)
    {
        if (Instance == null) throw new System.Exception("Mike added this: start the game from the LOADING SCENE to be able to change scenes.");
        Instance.InternalLoadScene(newScene);
    }

    private void InternalLoadScene(SceneIndexes newScene)
    {
        if (newScene == SceneIndexes.LOADING) throw new System.Exception("Can't load the load scene dummy, it loads itself.");
        if (runningOperations.Count != 0) throw new System.Exception("Cannont change screens while already loading");

        loadingScreen.gameObject.SetActive(true);

        runningOperations.Add(SceneManager.UnloadSceneAsync((int)currentScene));
        runningOperations.Add(SceneManager.LoadSceneAsync((int)newScene, LoadSceneMode.Additive));

        currentScene = newScene;

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

        loadingScreen.gameObject.SetActive(false);
    }
}

public enum SceneIndexes
{
    LOADING = 0,
    TITLE = 1,   
    BUILD = 2,
    DASHBOARD = 3
}
