using GMSpace;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Keypad : MonoBehaviour
{
    [SerializeField] private List<KeyInput> _keyInputs = new ();
    [SerializeField] private List<TextInput> _textInputs = new ();
    [SerializeField] private List<TextPassword> _textPasswords = new();

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
                key.OnClick?.Invoke(_textInputs[0].textComponent);
                CompareTexts();
                break;
            }
        }
    }

    public void AddTextInput(BaseKeypadText elementToAdd)
    {
        switch (elementToAdd)
        {
            case TextPassword textPassword:
                _textPasswords.Add(textPassword);
                _textPasswords = _textPasswords.OrderBy(e => e.id).ToList();
                break;
            case TextInput textInput:
                _textInputs.Add(textInput);
                _textInputs = _textInputs.OrderBy(e => e.id).ToList();
                break;
        }
    }

    private bool CompareTexts()
    {
        if (_textInputs[0].textComponent.text.Length == _textPasswords[0].numberOfChar)
        {
            if(_textInputs[0].textComponent.text == _textPasswords[0].textComponent.text)
            {
                return true;
            }
            else return false;
        }
        return false;
    }
}
