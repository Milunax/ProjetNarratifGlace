using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Runtime.InteropServices.WindowsRuntime;

public class CircuitBreaker : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] bool _isOn = true;
    [SerializeField] RectTransform _toggleIndicator;
    [SerializeField] Image _backgroundImg;
    [SerializeField] Color _colorOn;
    [SerializeField] Color _colorOff;
    [SerializeField] float _tweenTime = .25f;

    private float _onY;
    private float _offY;

    public bool IsOn { get => _isOn; }
    public delegate void ValueChanged(bool value);
    public event ValueChanged OnValueChanged;

    void Start()
    {
        _onY = _toggleIndicator.anchoredPosition.y;
        _offY = -_toggleIndicator.anchoredPosition.y;
        Toggle(_isOn);
    }

    void Toggle(bool value)
    {
        _isOn = !_isOn;
            Debug.Log("ça passe");
            ToggleColor(_isOn);
            MoveIndicator(_isOn);

            if(OnValueChanged != null)
                OnValueChanged(_isOn);
        
    }

    void ToggleColor(bool value)
    {
        if(value)
            _backgroundImg.DOColor(_colorOn, _tweenTime);
        else
            _backgroundImg.DOColor(_colorOff, _tweenTime);
    }

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
