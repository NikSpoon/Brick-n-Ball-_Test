using UnityEngine;

public class FinishUi : MonoBehaviour
{
    public void GoToMaimMenu()
    {
        Context.Instance.AppSystem.Trigger(AppTriger.ToMainMenu);
    }
}
