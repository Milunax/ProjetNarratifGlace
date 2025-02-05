using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PasswordManager : MonoBehaviour
{
    [SerializeField] private List<TextInput> _textInputs = new();
    [SerializeField] private List<TextPassword> _textPasswords = new();

    private Keypad _keypad;

    private void OnEnable()
    {
        Keypad.OnKeyPressed += UpdatePassword;
    }
    private void OnDisable()
    {
        Keypad.OnKeyPressed -= UpdatePassword;
    }

    public void AddTextToList(BaseKeypadText elementToAdd)
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

    public void RemoveTextFromList(BaseKeypadText elementToRemove)
    {
        switch (elementToRemove)
        {
            case TextPassword textPassword:
                _textPasswords.Remove(textPassword);
                _textPasswords = _textPasswords.OrderBy(e => e.id).ToList();
                break;
            case TextInput textInput:
                _textInputs.Remove(textInput);
                _textInputs = _textInputs.OrderBy(e => e.id).ToList();
                break;
        }
    }

    private bool CompareTexts(TextInput textInput, TextPassword textPassword)
    {
        if (textInput.textComponent.text.Length == textPassword.numberOfChar)
        {
            if (textInput.textComponent.text == textPassword.textComponent.text)
            {
                return true;
            }
            else
            {
                textInput.ResetText();
                return false;
            }
        }
        return false;
    }

    private void UpdatePassword(char charToAdd)
    {
        if (_textInputs.Count > 0 && _textPasswords.Count > 0)
        {
            _textInputs[0].textComponent.text += charToAdd;

            if (CompareTexts(_textInputs[0], _textPasswords[0]))
            {
                RemoveTextFromList(_textInputs[0]);
                RemoveTextFromList(_textPasswords[0]);
            }
        }
    }
}
