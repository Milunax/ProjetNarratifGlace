using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

public class CircuitBreaker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Switchs> _switchs = new List<Switchs>();
    [SerializeField] private List<Image> _ledList = new List<Image>();

    [Header("Code")]
    [SerializeField, ReadOnly] private string _code = "0977";
    [SerializeField] private TextMeshProUGUI _textCode;
    [SerializeField] private Image _imageCode;
    private string _entryCode;
    private bool _codeActive = false;

    [Header("Parameters")]
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _blockedColor;
    [SerializeField] private Color _defaultColor;

    private int _cursor = 0;

    void OnEnable()
    {
        DirectionalPad.OnKeyPressed += ChangeSwitchSelection;
        Keypad.OnKeyPressed += CheckCode;
    }
    void OnDisable()
    {
        DirectionalPad.OnKeyPressed -= ChangeSwitchSelection;
        Keypad.OnKeyPressed -= CheckCode;
    }

    private void Start()
    {
        _cursor = 0;

        _imageCode.gameObject.SetActive(false);
        _textCode.gameObject.SetActive(false);
        _codeActive = false;
}

    private void ChangeSwitchSelection(DIRECTIONAL_PAD_INFO info)
    {
        if (_codeActive) return;

        _ledList[_cursor].material.color = _defaultColor;

        switch (info)
        {
            case DIRECTIONAL_PAD_INFO.LEFT:
                {
                    _cursor--;
                    if (_cursor < 0) _cursor = _switchs.Count - 1;

                    if (_switchs[_cursor].GetSetIsLocked) _ledList[_cursor].material.color = _blockedColor;
                    else _ledList[_cursor].material.color = _selectedColor;

                    break;
                }
            case DIRECTIONAL_PAD_INFO.RIGHT:
                {
                    _cursor++;
                    if (_cursor >= _switchs.Count) _cursor = 0;

                    if (_switchs[_cursor].GetSetIsLocked) _ledList[_cursor].material.color = _blockedColor;
                    else _ledList[_cursor].material.color = _selectedColor;

                    break;
                }

            case DIRECTIONAL_PAD_INFO.CONFIRM:
                {
                    if (!_switchs[_cursor].GetSetIsLocked) _switchs[_cursor].Toggle();
                    else if (!_switchs[_cursor].GetSetIsLocked && _switchs[_cursor].GetSwitchType == SWITCHS.AI)
                    {
                        EnterCode();
                    }

                    break;
                }
    
        }
    }

    private void EnterCode()
    {
        _codeActive = true;

        _imageCode.gameObject.SetActive(true);
        _textCode.gameObject.SetActive(true);

        _entryCode = "";
        _textCode.text = _entryCode;
    }
    private void CheckCode(char num)
    {
        if (!_codeActive) return;

        _entryCode += num;
        _textCode.text = _entryCode;

        if (_entryCode.Length >= _code.Length)
        {
            if (_entryCode == _code)
            {
                _switchs[_cursor].GetSetIsLocked = false;
                _ledList[_cursor].color = _selectedColor;
            }

            _imageCode.gameObject.SetActive(false);
            _textCode.gameObject.SetActive(false);

            _codeActive = false;
            _entryCode = "";
        }
    }

}
