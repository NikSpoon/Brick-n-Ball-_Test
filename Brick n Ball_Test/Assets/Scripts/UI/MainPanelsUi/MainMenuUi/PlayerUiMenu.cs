using TMPro;
using UnityEngine;

public class PlayerUiMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _levl;


    private void OnEnable()
    {
        Context.Instance.PlayerProf.OnLevlChenged += OnLevlChenged;
    }
    private void OnDisable()
    {
        Context.Instance.PlayerProf.OnLevlChenged -= OnLevlChenged;
    }
    private void Start()
    {
        _name.text = Context.Instance.PlayerProf.Name;
        _levl.text = Context.Instance.PlayerProf.Levl.ToString();
    }
    private void OnLevlChenged(int value)
    {
        _levl.text = $"{value}";
    }
}
