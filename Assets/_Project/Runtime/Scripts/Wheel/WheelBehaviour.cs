using GMSpace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using System;
using UnityEngine.UI;

public class WheelBehaviour : MonoBehaviour
{
    [Header("References")]
    private Shader _waveMaterial;
    [SerializeField] GameObject _waveImage;
    CanvasRenderer _canvaRenderer;

    [Header("Parameters")]
    [SerializeField] float _minWheelVal = 0.0f;
    [SerializeField] float _maxWheelVal = 100.0f;
    [SerializeField] float _multiplyFactor = 1f;
    [SerializeField] TMP_Text _scrolltext;

    private Coroutine _UpdateWheel;
    private bool _isActive;

    void Start()
    {
        GameManager.playerInputs.primaryTouch.action.started += OnFingerSlideStarted;
        GameManager.playerInputs.primaryTouch.action.canceled += OnFingerSlideEnded;

        GameManager.Instance.SetWheelMinMax(_minWheelVal, _maxWheelVal);
        _isActive = GameManager.Instance.GetSetWaveValidity;

        var shader = Shader.Find("Custom/SineWave");
        _waveMaterial = _waveImage.GetComponent<Shader>();
        _waveMaterial = shader;
        _canvaRenderer = _waveImage.GetComponent<CanvasRenderer>();


    }

    private void OnDisable()
    {
        GameManager.playerInputs.primaryTouch.action.started -= OnFingerSlideStarted;
        GameManager.playerInputs.primaryTouch.action.canceled -= OnFingerSlideEnded;
    }

    void OnFingerSlideStarted(InputAction.CallbackContext ctx)
    {
        GameObject temp = GameManager.playerInputs.Detection();
        if (temp != null && temp == gameObject)
        {
            _UpdateWheel = StartCoroutine(UpdateWheel());
        }
    }

    void OnFingerSlideEnded(InputAction.CallbackContext ctx)
    {
        if (_UpdateWheel != null) StopCoroutine(_UpdateWheel);
    }

    IEnumerator UpdateWheel()
    {
        float startValue = GameManager.Instance.GetSetWheelValue;

        while (true)
        {
            _scrolltext.text = GameManager.Instance.GetSetWheelValue.ToString();
            GameManager.Instance.GetSetWheelValue = startValue + (GameManager.playerInputs.GetSlideDeltaV.y * _multiplyFactor);

            _canvaRenderer.GetMaterial().SetFloat("_Wave1Frequency", GameManager.Instance.GetSetWheelValue/_maxWheelVal * 10);
            yield return new WaitForFixedUpdate();
        }
    }
}
