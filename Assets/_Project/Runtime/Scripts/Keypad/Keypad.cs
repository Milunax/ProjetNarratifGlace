using GMSpace;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Keypad : MonoBehaviour
{
    [SerializeField] private List<KeyInput> _keyInputs = new ();

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
                key.OnClick?.Invoke();
                break;
            }
        }
    }
}
