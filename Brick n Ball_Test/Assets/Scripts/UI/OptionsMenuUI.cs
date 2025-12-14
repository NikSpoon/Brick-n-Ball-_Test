using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider _musicSlider; 
    [SerializeField] private Button _lowBtn;
    [SerializeField] private Button _midBtn;
    [SerializeField] private Button _ultraBtn;

    [Header("Slider Mode")]
    [SerializeField] private bool _sliderIsPercent = false; 

    private AudioSource _musicSource;
    private bool _ignoreSliderEvent;

    private void Awake()
    {
       
        if (_lowBtn) _lowBtn.onClick.AddListener(() => GameSettingsSingleton.Instance?.SetLowPerformance());
        if (_midBtn) _midBtn.onClick.AddListener(() => GameSettingsSingleton.Instance?.SetMediumPerformance());
        if (_ultraBtn) _ultraBtn.onClick.AddListener(() => GameSettingsSingleton.Instance?.SetHighPerformance());

        
        if (_musicSlider)
            _musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
    }

    private void Start()
    {
        BindToSingleton();
        PullValuesToUI();
    }

    private void OnEnable()
    {
        
        BindToSingleton();
        PullValuesToUI();
    }

    private void BindToSingleton()
    {
        var gs = GameSettingsSingleton.Instance;
        if (gs == null) return;

        _musicSource = gs.GetComponent<AudioSource>();
    }

    private void PullValuesToUI()
    {
        if (_musicSlider == null || _musicSource == null) return;

        _ignoreSliderEvent = true;

        float v = Mathf.Clamp01(_musicSource.volume);

        if (_sliderIsPercent)
            _musicSlider.value = v * 100f;   
        else
            _musicSlider.value = v;         

        _ignoreSliderEvent = false;
    }

    private void OnMusicSliderChanged(float value)
    {
        if (_ignoreSliderEvent) return;
        if (_musicSource == null) BindToSingleton();
        if (_musicSource == null) return;

        float v = _sliderIsPercent ? Mathf.Clamp01(value / 100f) : Mathf.Clamp01(value);
        _musicSource.volume = v;
    }
}
