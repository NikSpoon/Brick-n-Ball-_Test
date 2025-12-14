using TMPro;
using UnityEngine;

public class FinishDataUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _oldLevl;
    [SerializeField] private TextMeshProUGUI _newLevl;
    [SerializeField] private TextMeshProUGUI _levlScore;
    [SerializeField] private TextMeshProUGUI _Score;

    private void Start()
    {
        var ctx = Context.Instance.FinishRunData;

        _oldLevl.text = ctx.StartLevl.ToString();
        _newLevl.text = ctx.FinishLevl.ToString();
        _levlScore.text = ctx.LevlScore.ToString();
        _Score.text = ctx.Score.ToString();
    }
}
