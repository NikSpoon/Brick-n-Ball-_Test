
using System.Collections;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppControler : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (FindObjectsByType<AppControler>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

    }
    private void OnEnable()
    {
        Context.Instance.AppSystem.OnStateChange += OnStateChange;
    }
    private void OnDisable()
    {
        Context.Instance.AppSystem.OnStateChange -= OnStateChange;
    }
    private void OnStateChange(StateChangeData<AppState, AppTriger> data)
    {
        switch (data.NewState)
        {
            case AppState.MainMenu:
                StartCoroutine(ReloadSceneAsync("MainMenu", AppState.MainMenu));
                break;

            case AppState.Game:
                StartCoroutine(ReloadSceneAsync("Game", AppState.Game));
                break;

            case AppState.Finish:
                StartCoroutine(ReloadSceneAsync("Finish", AppState.Finish));
                break;

            default:
                break;
        }
    }

    private IEnumerator ReloadSceneAsync(string newScene, AppState state)
    {
        UiApp ui = FindFirstObjectByType<UiApp>();
        if (ui != null)
            ui.AddLoaderPanel(state);

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);

        while (!loadOp.isDone)
            yield return null;

        Scene loadedScene = SceneManager.GetSceneByName(newScene);
        if (loadedScene.IsValid())
            SceneManager.SetActiveScene(loadedScene);

        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount - 1; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != newScene && scene.name != "UIScene")
            {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(scene);
                while (!unloadOp.isDone)
                    yield return null;
            }
        }
        
    

        if (ui != null)
        {
            ui.ReloadUI();
            ui.RemuveLoaderPanel();
        }
    }
}

