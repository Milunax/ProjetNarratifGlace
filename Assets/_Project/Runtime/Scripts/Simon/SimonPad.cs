using GMSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class SimonPad : MonoBehaviour
{
    [SerializeField] private List<SimonButtons> _buttonsList = new();

    public static event Action<SIMON_PAD_INFO> OnKeyPressed;

    private void Start()
    {
        GameManager.playerInputs.primaryTouch.action.started += CheckButtonPressed;
    }

    private void OnDisable()
    {
        GameManager.playerInputs.primaryTouch.action.started -= CheckButtonPressed;
    }

    private void CheckButtonPressed(CallbackContext ctx)
    {
        var goDetected = GameManager.playerInputs.Detection();
        foreach (SimonButtons button in _buttonsList)
        {
            Debug.Log("FOR");
            if (goDetected == button.gameObject)
            {
                Debug.Log("DETECTED");
                if (goDetected.TryGetComponent(out SimonButtons foundComponent))
                {
                    SIMON_PAD_INFO inputInfo = foundComponent.PressedInput();
                    OnKeyPressed?.Invoke(inputInfo);
                    break;
                }
            }
        }
    }
}
