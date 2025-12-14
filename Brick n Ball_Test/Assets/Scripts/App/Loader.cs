using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
    }
    private void Start()
    {
        
        var settings = GameSettingsSingleton.Instance;
        if (settings != null)
        {
            settings.SetMediumPerformance();

            var audio = settings.GetComponent<AudioSource>();
            if (audio != null)
                audio.volume = 0.1f; // 50%
        }

        Context.Instance.AppSystem.Trigger(AppTriger.ToMainMenu);
        Context.Instance.PlayerProf.InitTestProf();
    }
}
