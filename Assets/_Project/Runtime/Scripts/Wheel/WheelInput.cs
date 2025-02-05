using GMSpace;
using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class WheelInput : MonoBehaviour
{
    public static event Action<bool> OnKeyPressed;
    private bool _wasInput;

    private void Start()
    {
        GameManager.playerInputs.primaryTouch.action.started += CheckButtonPressed;
        GameManager.playerInputs.primaryTouch.action.canceled += CheckButtonUnpressed;
    }

    private void OnDisable()
    {
        GameManager.playerInputs.primaryTouch.action.started -= CheckButtonPressed;
        GameManager.playerInputs.primaryTouch.action.canceled -= CheckButtonUnpressed;
    }

    private void CheckButtonPressed(CallbackContext ctx)
    {
        var goDetected = GameManager.playerInputs.Detection();

        if (goDetected == gameObject)
        {
            _wasInput = true;
            OnKeyPressed?.Invoke(true);
        }
    }
    private void CheckButtonUnpressed(CallbackContext ctx)
    {
        if (_wasInput)
        {
            _wasInput = false;
            OnKeyPressed?.Invoke(false);
        }
    }
}
