using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices.WindowsRuntime;
using GMSpace;

public class CircuitBreaker : MonoBehaviour, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] RectTransform _toggleIndicator;
    [SerializeField] Image _backgroundImg;
    [SerializeField] Color _colorOn;
    [SerializeField] Color _colorOff;

    [Header("Parameters")]
    [SerializeField] bool _isOn = true;
    [SerializeField] bool _isLocked = false;
    [SerializeField] float _tweenTime = .25f;

    private float _onY;
    private float _offY;

    public bool IsOn { get => _isOn; }
    public delegate void ValueChanged(bool value);
    public event ValueChanged OnValueChanged;

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
        if(_isLocked == false)
        {
            _isOn = !_isOn;
            //Debug.Log("ça passe");
            ToggleColor(_isOn);
            MoveIndicator(_isOn);

            if(OnValueChanged != null)
                OnValueChanged(_isOn);
        }
    }

    //---- Color switching ----//
    void ToggleColor(bool value)
    {
        if(value)
            _backgroundImg.DOColor(_colorOn, _tweenTime);
        else
            _backgroundImg.DOColor(_colorOff, _tweenTime);
    }

    //---- Animation of the switches ----//
    void MoveIndicator (bool value)
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
