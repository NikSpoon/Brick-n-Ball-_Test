
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerControl : MonoBehaviour
{
    public InputActionAsset MyInputAction;

    private InputAction _moveAction;
    private InputAction _jumpAction;

    [Header("Movement")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpForce = 5f;

    private void OnEnable()
    {
        MyInputAction.Enable();
    }
    private void OnDisable()
    {
        MyInputAction.Disable();
    }

    private void Awake()
    {
        var map = MyInputAction.FindActionMap("GamePlay", throwIfNotFound: true);
        _moveAction = map.FindAction("Movement", throwIfNotFound: true);
        _jumpAction = map.FindAction("Jump", throwIfNotFound: true);
    }
    private void Update()
    {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        Move(moveInput);

        if (_jumpAction.WasPressedThisDynamicUpdate())
        {
            Jump();

        }
    }
    private void Move(Vector2 moveInput)
    {
        // X = влево/вправо, Y = вперёд/назад
        var dir = new Vector3(moveInput.x, 0f, moveInput.y);

        // Простейший вариант через Translate (для теста ок)
        transform.Translate(dir * _speed * Time.deltaTime, Space.World);

        // Если хочешь через Rigidbody:
        // Vector3 velocity = dir * _speed;
        // velocity.y = _rigidbody.velocity.y;
        // _rigidbody.velocity = velocity;
    }

    private void Jump()
    {
        if (_rigidbody == null) return;

        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }
}
