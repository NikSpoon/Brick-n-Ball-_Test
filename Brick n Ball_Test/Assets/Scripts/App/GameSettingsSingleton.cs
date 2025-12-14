using UnityEngine;

public class GameSettingsSingleton : MonoBehaviour
{
    public static GameSettingsSingleton Instance { get; private set; }

    [Header("Music Clips")]
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _gameMusic;
    [SerializeField] private AudioClip _finishMusic;

    private AudioSource _musicSource;
    private AppState _lastState;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _musicSource = GetComponent<AudioSource>();
        if (_musicSource == null)
            _musicSource = gameObject.AddComponent<AudioSource>();

        _musicSource.loop = true;
        _musicSource.playOnAwake = false;

        _lastState = AppState.Loading;
    }

    private void Update()
    {
        var ctx = Context.Instance;
        if (ctx == null) return;

        var current = ctx.AppSystem.CurrentState;
        if (current == _lastState) return;

        SwitchMusic(current);
        _lastState = current;
    }

    private void SwitchMusic(AppState state)
    {
        AudioClip target = null;

        switch (state)
        {
            case AppState.MainMenu: target = _menuMusic; break;
            case AppState.Game: target = _gameMusic; break;
            case AppState.Finish: target = _finishMusic; break;
        }

        if (target == null) return;
        if (_musicSource.clip == target && _musicSource.isPlaying) return;

        _musicSource.clip = target;
        _musicSource.Play();
    }

    public void SetLowPerformance()
    {
        Application.targetFrameRate = 30;
        QualitySettings.SetQualityLevel(0, true);
    }

    public void SetMediumPerformance()
    {
        Application.targetFrameRate = 60;
        QualitySettings.SetQualityLevel(QualitySettings.names.Length / 2, true);
    }

    public void SetHighPerformance()
    {
        Application.targetFrameRate = -1;
        QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1, true);
    }
}
