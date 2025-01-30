using GMSpace;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

public class DirectionalPadButton : MonoBehaviour
{
    [SerializeField] public int selectedMethodId;
    [SerializeField] public Action<CallbackContext> OnClick;

    public event Action<DIRECTIONAL_PAD_INFO> OnButtonPressed;

    private void Awake()
    {
        var _methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        OnClick = (c) => _methods[selectedMethodId].Invoke(this, new object[] { c });

        //GameManager.playerInputs.primaryTouch.action.started += (c) => OnClick.Invoke(c);
    }

    private void OnDisable()
    {
        //GameManager.playerInputs.primaryTouch.action.started -= (c) => OnClick.Invoke(c);
    }

    public DIRECTIONAL_PAD_INFO Confirm(CallbackContext c)
    {
        Debug.Log(DIRECTIONAL_PAD_INFO.CONFIRM.ToString());
        OnButtonPressed?.Invoke(DIRECTIONAL_PAD_INFO.CONFIRM);
        return DIRECTIONAL_PAD_INFO.CONFIRM;
    }

    public DIRECTIONAL_PAD_INFO GoUp(CallbackContext c)
    {
        Debug.Log(DIRECTIONAL_PAD_INFO.UP.ToString());
        OnButtonPressed?.Invoke(DIRECTIONAL_PAD_INFO.UP);
        return DIRECTIONAL_PAD_INFO.UP;
    }

    public DIRECTIONAL_PAD_INFO GoDown(CallbackContext c)
    {
        Debug.Log(DIRECTIONAL_PAD_INFO.DOWN.ToString());
        OnButtonPressed?.Invoke(DIRECTIONAL_PAD_INFO.DOWN);
        return DIRECTIONAL_PAD_INFO.DOWN;
    }

    public DIRECTIONAL_PAD_INFO GoRight(CallbackContext c)
    {
        Debug.Log(DIRECTIONAL_PAD_INFO.RIGHT.ToString());
        OnButtonPressed?.Invoke(DIRECTIONAL_PAD_INFO.RIGHT);
        return DIRECTIONAL_PAD_INFO.RIGHT;
    }

    public DIRECTIONAL_PAD_INFO GoLeft(CallbackContext c)
    {
        Debug.Log(DIRECTIONAL_PAD_INFO.LEFT.ToString());
        OnButtonPressed?.Invoke(DIRECTIONAL_PAD_INFO.LEFT);
        return DIRECTIONAL_PAD_INFO.LEFT;
    }
}
