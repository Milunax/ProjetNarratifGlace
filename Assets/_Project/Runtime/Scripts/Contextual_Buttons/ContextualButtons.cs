using GMSpace;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class ContextualButtons : MonoBehaviour
{
    [SerializeField] private List<OtherButtons> _buttonsInput = new();

    public static event Action<CONTEXTUAL_INPUT_INFO> OnKeyPressed;

    // Start is called before the first frame update
    void Start()
    {
        _buttonsInput.AddRange(GetComponentsInChildren<OtherButtons>());

        GameManager.playerInputs.primaryTouch.action.started += CheckKeyPressed;
    }

    private void OnDisable()
    {
        GameManager.playerInputs.primaryTouch.action.started -= CheckKeyPressed;
    }

    private void CheckKeyPressed(CallbackContext ctx)
    {
        var goDetected = GameManager.playerInputs.Detection();
        foreach (OtherButtons button in _buttonsInput)
        {
            if (goDetected == button.gameObject)
            {
                OnKeyPressed?.Invoke(button.Clicked());
                break;
            }
        }
    }
}
