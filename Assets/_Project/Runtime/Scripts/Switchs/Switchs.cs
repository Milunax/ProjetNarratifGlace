using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices.WindowsRuntime;
using GMSpace;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Switchs : MonoBehaviour, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private Switchs _previousSwitchs;
    [SerializeField] private Switchs _nextSwitchs;
    [SerializeField] RectTransform _toggleIndicator;
    [SerializeField] Image _backgroundImg;
    [SerializeField] Color _colorOn;
    [SerializeField] Color _colorOff;

    [Header("Parameters")]
    //[SerializeField] private int _column = 3;
    [SerializeField] private bool _isSelectable = true;
    [SerializeField] bool _isOn = true;
    [SerializeField] bool _isLocked = false;
    [SerializeField] float _tweenTime = .25f;

    private float _onY;
    private float _offY;

    public bool IsOn { get => _isOn; }
    public delegate void ValueChanged(bool value);
    public event ValueChanged OnValueChanged;

    //public int Column { get => _column; set => _column = value; }
    public Switchs Previous { get { return _previousSwitchs; } set { _previousSwitchs = value; } }
    public Switchs NextSwitch { get { return _nextSwitchs; } set { _nextSwitchs = value; } }
    public bool selection { get => _isSelectable; set => _isSelectable = value; }

    void Start()
    {
        GameManager.playerInputs.primaryTouch.action.started += OnFingerSlideStarted;

        _onY = _toggleIndicator.anchoredPosition.y;
        _offY = -_toggleIndicator.anchoredPosition.y;
    }

    void OnDisable()
    {
        GameManager.playerInputs.primaryTouch.action.started -= OnFingerSlideStarted;
    }

    void OnFingerSlideStarted(InputAction.CallbackContext ctx)
    {
        GameObject temp = GameManager.playerInputs.Detection();
        if (temp != null && temp == gameObject)
        {
            //Debug.Log("DD");
            Toggle(_isOn);
        }
    }

    //---- Activation ----//
    void Toggle(bool value)
    {
        if (_isLocked == false)
        {
            _isOn = !_isOn;
            //Debug.Log("ça passe");
            ToggleColor(_isOn);
            MoveIndicator(_isOn);

            if (OnValueChanged != null)
                OnValueChanged(_isOn);
        }
    }

    //---- Color switching ----//
    void ToggleColor(bool value)
    {
        if (value)
            _backgroundImg.DOColor(_colorOn, _tweenTime);
        else
            _backgroundImg.DOColor(_colorOff, _tweenTime);
    }

    //---- Animation of the switches ----//
    void MoveIndicator(bool value)
    {
        if (value)
            _toggleIndicator.DOAnchorPosY(_onY, _tweenTime);
        else
            _toggleIndicator.DOAnchorPosY(_offY, _tweenTime);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Toggle(_isOn);
    }
}
