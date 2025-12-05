using UnityEngine;

public class MainMenuUiPanel : MonoBehaviour
{
    [SerializeField] private GameObject _options;
    public void PLay()
    {
        Context.Instance.AppSystem.Trigger(AppTriger.ToGame);
    }
    public void Options()
    {
        if (!_options.activeSelf && _options != null)
        {
            _options.SetActive(true);
        }
    }
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
