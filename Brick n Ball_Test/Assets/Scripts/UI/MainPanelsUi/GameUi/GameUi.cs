using UnityEngine;

public class GameUi : MonoBehaviour
{
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _finishPanel;

    public void GoToFinish()
    {
        Context.Instance.AppSystem.Trigger(AppTriger.ToFinish);
    }
    public void OpenMenu()
    {
        OpenPanelChek(_options);
    }
    public void CloseMenu()
    {
        ClosePanelChek(_options);
    }
    public void FinishGame()
    {
        OpenPanelChek(_finishPanel);
    }
    private void OpenPanelChek(GameObject panel)
    {
        if (panel != null && !panel.activeSelf)
        {
            panel.SetActive(true);
        }
    }
    private void ClosePanelChek(GameObject panel)
    {
        if (panel != null && panel.activeSelf)
        {
            panel.SetActive(false);
        }
    }
}
