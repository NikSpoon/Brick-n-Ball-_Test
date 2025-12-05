using UnityEngine;

public class Loader : MonoBehaviour
{
    private void Start()
    {
        Context.Instance.AppSystem.Trigger(AppTriger.ToMainMenu);
    }
}
