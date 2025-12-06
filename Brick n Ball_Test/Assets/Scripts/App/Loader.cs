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
        Context.Instance.AppSystem.Trigger(AppTriger.ToMainMenu);
    }
}
