using GMSpace;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

public class DirectionalPad : MonoBehaviour
{
    [SerializeField] public int selectedMethodId;
    [SerializeField] public Action<CallbackContext> OnConfirm = delegate { };

    private void Start()
    {
        GameManager.playerInputs.primaryTouch.action.started += (c) => OnConfirm.Invoke(c);
    }

    private void OnDisable()
    {
        GameManager.playerInputs.primaryTouch.action.started -= (c) => OnConfirm.Invoke(c);
    }

    public void Confirm(CallbackContext c)
    {
        Debug.Log(c.ToString());
    }

    public void GoUp(CallbackContext c)
    {

    }

    public void GoDown(CallbackContext c)
    {

    }

    public void GoRight(CallbackContext c)
    {

    }

    public void GoLeft(CallbackContext c)
    {

    }
}
