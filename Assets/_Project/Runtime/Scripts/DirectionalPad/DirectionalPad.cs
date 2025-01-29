using GMSpace;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

public class DirectionalPad : MonoBehaviour
{
    [SerializeField] private List<GameObject> _buttonsList = new ();

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
        foreach(GameObject gameObject in _buttonsList)
        {
            if (goDetected == gameObject)
            {
                if (goDetected.TryGetComponent(out DirectionalPadButton foundComponent))
                {
                    foundComponent.OnClick?.Invoke(ctx);
                    break;
                }
            }
        }
    }
}
