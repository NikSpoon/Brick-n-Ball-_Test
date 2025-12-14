using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator _animator;

    [Header("Input")]
    [SerializeField] private InputActionReference _movement;
    [SerializeField] private InputActionReference _jump;
    [SerializeField] private InputActionReference _attack;

    [Header("Smoothing")]
    [SerializeField] private float damp = 0.08f;

    [Header("Deadzone")]
    [SerializeField] private float deadzone = 0.01f;

    void OnEnable()
    {
        _movement?.action.Enable();
        _jump?.action.Enable();
        _attack?.action.Enable();
    }

    void OnDisable()
    {
        _movement?.action.Disable();
        _jump?.action.Disable();
        _attack?.action.Disable();
    }

    void Update()
    {
        if (_animator == null || _movement == null)
            return;

        Vector2 move = _movement.action.ReadValue<Vector2>();
        bool isMoving = move.sqrMagnitude > deadzone * deadzone;

        move = Vector2.ClampMagnitude(move, 1f);
        _animator.SetFloat("MoveX", move.x, damp, Time.deltaTime);
        _animator.SetFloat("MoveY", move.y, damp, Time.deltaTime);
        _animator.SetBool("IsWalking", isMoving);

        bool jumpPulse = _jump != null && _jump.action.triggered;
        bool attackPulse = _attack != null && _attack.action.triggered;

        _animator.SetBool("Jump", jumpPulse);

        if (attackPulse)
            _animator.SetTrigger("Attack");
    }
}