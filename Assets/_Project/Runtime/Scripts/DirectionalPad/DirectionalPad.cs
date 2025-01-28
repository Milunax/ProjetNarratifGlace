using GMSpace;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

public class DirectionalPad : MonoBehaviour
{
    [SerializeField] public Action<CallbackContext> OnConfirm = delegate { };

    private void Start()
    {
        GameManager.playerInputs.primaryTouch.action.started += (c) => OnConfirm.Invoke(c);
    }

    private void OnDisable()
    {
        GameManager.playerInputs.primaryTouch.action.started -= (c) => OnConfirm.Invoke(c);
    }

    public void AZERTY(CallbackContext c)
    {
        Debug.Log(c.ToString());
    }
}
