using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppControler : MonoBehaviour
{
    [SerializeField] private UiApp _uiApp;
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
                StartCoroutine(ReloadSceneAsync("Game3D", AppState.Game));
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
        
        _uiApp.StartReloadUI(state);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(newScene);
       
        while (!loadOperation.isDone)
        {
            
            yield return null;
        }
       
        _uiApp.ReloadUI();
    }
}

