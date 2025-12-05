using UnityEngine;

public class GameFinishUi : MonoBehaviour
{
    public void Yes()
    {
        gameObject.SetActive(false);
    }
    public void No()
    {
        Context.Instance.AppSystem.Trigger(AppTriger.ToFinish);
    }
}
