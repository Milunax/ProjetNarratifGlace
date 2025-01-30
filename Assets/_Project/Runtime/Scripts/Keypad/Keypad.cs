using GMSpace;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Keypad : MonoBehaviour
{
    [SerializeField] private List<KeyInput> _keyInputs = new ();

    public Action<char> OnKeyPressed;

    // Start is called before the first frame update
    void Start()
    {
        _keyInputs.AddRange(GetComponentsInChildren<KeyInput>());

        GameManager.playerInputs.primaryTouch.action.started += CheckKeyPressed;
    }

    private void OnDisable()
    {
        GameManager.playerInputs.primaryTouch.action.started -= CheckKeyPressed;
    }

    private void CheckKeyPressed(CallbackContext ctx)
    {   
        var goDetected = GameManager.playerInputs.Detection();
        foreach (KeyInput key in _keyInputs)
        {
            if (goDetected == key.gameObject)
            {
                OnKeyPressed?.Invoke(key.Clicked());
                break;
            }
        }
    }
}
