using TMPro;
using UnityEngine;

public class SessionUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _levl;
    [SerializeField] private TextMeshProUGUI _score;

    
    private void OnEnable()
    {
        Context.Instance.PlayerProf.OnLevlChenged += OnLevlChenged;
        Context.Instance.SessionData.OnScoreChenged += OnScoreChenged;
    }
    private void OnDisable()
    {
        Context.Instance.PlayerProf.OnLevlChenged -= OnLevlChenged;
        Context.Instance.SessionData.OnScoreChenged -= OnScoreChenged;
    }
    private void Start()
    {
        _name.text = Context.Instance.PlayerProf.Name;
        _levl.text = Context.Instance.PlayerProf.Levl.ToString();
        _score.text = "0";
    }
    private void OnLevlChenged(int value)
    {
        _levl.text = $"{value}";
    }
    private void OnScoreChenged(int value)
    {
        _score.text = $"{value}";
    }
}
