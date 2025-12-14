using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMapSwitcher : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    private bool menuOpen;

    private InputActionMap gameplay;
    private InputActionMap ui;

    private void Awake()
    {
        gameplay = inputActions.FindActionMap("GamePlay", true);
        ui = inputActions.FindActionMap("UI", true);


        if (Context.Instance.AppSystem.CurrentState == AppState.Game)
        {
            gameplay.Enable();
            ui.Disable();
        }
        else
        {
            ui.Enable();
            gameplay.Disable();
        }
    }
    public void OpenMenu()
    {
        if (menuOpen) return;

        menuOpen = true;

        gameplay.Disable();
        ui.Enable();

        Time.timeScale = 0f; 
    }

    public void CloseMenu()
    {
        if (!menuOpen) return;

        menuOpen = false;

        ui.Disable();
        gameplay.Enable();

        Time.timeScale = 1f;
    }

    public void ToggleMenu()
    {
        if (menuOpen) CloseMenu();
        else OpenMenu();
    }
}
