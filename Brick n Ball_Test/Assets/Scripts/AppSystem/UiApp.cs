using UnityEngine;

public class UiApp : MonoBehaviour
{
    [SerializeField] private Transform _root;

    [SerializeField] private GameObject _loaderPanel;
    [SerializeField] private GameObject _errorPanel;

    [SerializeField] private GameObject _loading;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _game;
    [SerializeField] private GameObject _finish;

    private GameObject _currentScreen;
    private GameObject _newScreen;
    private GameObject _currentLoaderPanel;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (FindObjectsByType<UiApp>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

    }
    private void OnEnable()
    {
        if (_currentScreen == null)
        {
            _currentScreen = Instantiate(_loading, _root);
        }
    }
    private void AppSystemStateChange(AppState state)
    {
        switch (state)
        {
            case AppState.MainMenu: _newScreen = _mainMenu; break;
            case AppState.Game: _newScreen = _game; break;
            case AppState.Finish: _newScreen = _finish; break;
            default: _newScreen = _errorPanel; break;
        }
    }

    public void StartReloadUI(AppState newState)
    {
        AppSystemStateChange(newState);

        if (_currentScreen != null)
            Destroy(_currentScreen);

        _currentLoaderPanel = Instantiate(_loaderPanel, _root.transform);
        _currentScreen = Instantiate(_newScreen, _root.transform);
    }
    public void ReloadUI()
    {
        if (_currentLoaderPanel != null)
        {
            Destroy(_currentLoaderPanel);
            _currentLoaderPanel = null;
        }
    }

}
