using UnityEngine;

public class AudioListenerFix : MonoBehaviour
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
    private void LateUpdate()
    {
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);

        if (listeners.Length > 1)
        {
            for (int i = 1; i < listeners.Length; i++)
            {
                listeners[i].enabled = false;
            }
        }
    }
}
