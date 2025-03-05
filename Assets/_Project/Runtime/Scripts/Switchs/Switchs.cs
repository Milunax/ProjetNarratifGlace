using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Switchs : MonoBehaviour
{
    [Header("References")]
    [SerializeField] RectTransform _toggleIndicator;
    [SerializeField] Image _backgroundImg;
    [SerializeField] Color _colorOn;
    [SerializeField] Color _colorOff;

    [Header("Parameters")]
    [SerializeField] private SWITCHS _type;
    [SerializeField] private bool _isSelectable = true;
    [SerializeField] private bool _isOn = true;
    [SerializeField] private bool _isLocked = false;
    [SerializeField] private float _tweenTime = .25f;


    private float _onY;
    private float _offY;

    public bool GetIsOn { get => _isOn; }
    public SWITCHS GetSwitchType { get => _type; }
    public bool GetSetIsLocked { get => _isLocked; set => _isLocked = value; }
    public delegate void ValueChanged(bool value);
    public event ValueChanged OnValueChanged;

    private void Start()
    {
        _onY = _toggleIndicator.anchoredPosition.y;
        _offY = -_toggleIndicator.anchoredPosition.y;
    }

    //---- Activation ----//
    public void Toggle()
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
    private void ToggleColor(bool value)
    {
        if (value)
            _backgroundImg.DOColor(_colorOn, _tweenTime);
        else
            _backgroundImg.DOColor(_colorOff, _tweenTime);
    }

    //---- Animation of the switches ----//
    private void MoveIndicator(bool value)
    {
        if (value)
            _toggleIndicator.DOAnchorPosY(_onY, _tweenTime);
        else
            _toggleIndicator.DOAnchorPosY(_offY, _tweenTime);
    }
}
