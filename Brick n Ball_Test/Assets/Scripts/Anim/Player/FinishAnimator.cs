using UnityEngine;

public class FinishAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _winTrigger = "Win";
    [SerializeField] private string _loseTrigger = "Lose";

    private bool _played;

    private void OnEnable()
    {
        _played = false;
        PlayOnce();
    }

    private void Update()
    {
        // если Context ещё не успел заполниться — попробуем позже
        if (!_played) PlayOnce();
    }

    private void PlayOnce()
    {
        var ctx = Context.Instance;
        if (ctx == null) return;

        
        var reason = ctx.FinishRunData.Reason;

        if (reason == FinishReason.Win)
        {
            _animator.ResetTrigger(_loseTrigger);
            _animator.SetTrigger(_winTrigger);
            _played = true;
        }
        else if (reason == FinishReason.Lose)
        {
            _animator.ResetTrigger(_winTrigger);
            _animator.SetTrigger(_loseTrigger);
            _played = true;
        }
    }
}
