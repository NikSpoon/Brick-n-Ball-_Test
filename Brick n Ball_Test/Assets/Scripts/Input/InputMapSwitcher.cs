
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMapSwitcher : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;

    private bool _menuOpen;

    private InputActionMap _gameplay;
    private InputActionMap _ui;

    private void Awake()
    {
        _gameplay = _inputActions.FindActionMap("GamePlay", true);
        _ui = _inputActions.FindActionMap("UI", true);


        if (Context.Instance.AppSystem.CurrentState == AppState.Game)
        {
            _gameplay.Enable();
            _ui.Disable();
        }
        else
        {
            _ui.Enable();
            _gameplay.Disable();
        }
    }
    public void OpenMenu()
    {
        if (_menuOpen) return;

        _menuOpen = true;

        _gameplay.Disable();
        _ui.Enable();

        Time.timeScale = 0f; 
    }

    public void CloseMenu()
    {
        if (!_menuOpen) return;

        _menuOpen = false;

        _ui.Disable();
        _gameplay.Enable();

        Time.timeScale = 1f;
    }

    public void ToggleMenu()
    {
        if (_menuOpen) CloseMenu();
        else OpenMenu();
    }
}
