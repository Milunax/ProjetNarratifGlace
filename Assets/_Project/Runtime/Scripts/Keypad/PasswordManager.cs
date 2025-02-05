using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using UnityEngine;

public class PasswordManager : MonoBehaviour
{
    [SerializeField] private GameObject _taskContainer;

    [SerializeField] private List<TextInput> _textInputs = new();
    [SerializeField] private List<TextPassword> _textPasswords = new();
    [SerializeField] private int _currentPasswordId = 0;

    private Keypad _keypad;
    [SerializeField] private int _passwordToCompleteCount;
    [SerializeField] private int _passwordCompletedCount;


    private void OnEnable()
    {
        Keypad.OnKeyPressed += UpdatePassword;
    }
    private void OnDisable()
    {
        ResetTask();
        Keypad.OnKeyPressed -= UpdatePassword;
    }

    public void AddTextToList(BaseKeypadText elementToAdd)
    {
        switch (elementToAdd)
        {
            case TextPassword textPassword:
                _textPasswords.Add(textPassword);
                _textPasswords = _textPasswords.OrderBy(e => e.id).ToList();
                _passwordToCompleteCount++;
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
                _passwordCompletedCount++;
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
            _textInputs[_currentPasswordId].textComponent.text += charToAdd;

            if (CompareTexts(_textInputs[_currentPasswordId], _textPasswords[_currentPasswordId]))
            {
                _currentPasswordId++;
                //RemoveTextFromList(_textInputs[_currentPasswordId]);
                //RemoveTextFromList(_textPasswords[_currentPasswordId]);

                CheckForTaskEnd();
            }
        }
    }

    private void CheckForTaskEnd()
    {
        if(_passwordCompletedCount >= _passwordToCompleteCount)
        {
            FindObjectOfType<Map>().TaskFinished(_taskContainer);   
            ResetTask();
        }
    }

    private void ResetTask()
    {
        //remettre la task a son �tat de base
        _currentPasswordId = 0;
        _passwordCompletedCount = 0;
        foreach (var textInput in _textInputs)
        {
            textInput.textComponent.text = "";
        }

        foreach (var textPassword in _textPasswords)
        {
            textPassword.GeneratePassword();
        }
    }
}
