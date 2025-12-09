using UnityEngine;

public class Context : MonoBehaviour
{
    public static Context Instance { get; private set; }
   
    public IAppSystem AppSystem = new AppSystem();

    public PlayerProf PlayerProf = new PlayerProf();
    public SessionData SessionData = new SessionData();
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance == null)
        {
            var go = new GameObject("Context");
            Instance = go.AddComponent<Context>();
            DontDestroyOnLoad(go);
        }
    }

    private void OnDisable()
    {
        SessionData.ClearSession();
    }
}