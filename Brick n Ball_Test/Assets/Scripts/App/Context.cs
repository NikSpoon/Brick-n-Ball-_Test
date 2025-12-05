using UnityEngine;

public class Context : MonoBehaviour
{
    public static Context Instance { get; private set; }

    public IAppSystem AppSystem = new AppSystem();

   
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
}