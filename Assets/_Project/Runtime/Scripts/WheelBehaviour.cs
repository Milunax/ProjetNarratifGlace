using GMSpace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class WheelBehaviour : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]float _minWheelVal = 0.0f;
    [SerializeField]float _maxWheelVal = 100.0f;
    [SerializeField] float _multiplyFactor = 1f;

    private Coroutine _UpdateWheel;

    void Start()
    {
        GameManager.playerInputs.primaryTouch.action.started += OnFingerSlideStarted;
        GameManager.playerInputs.primaryTouch.action.canceled += OnFingerSlideEnded;

        GameManager.Instance.SetWheelMinMax(_minWheelVal, _maxWheelVal);
    }

    private void OnDisable()
    {
        GameManager.playerInputs.primaryTouch.action.started -= OnFingerSlideStarted;
        GameManager.playerInputs.primaryTouch.action.canceled -= OnFingerSlideEnded;
    }

    void OnFingerSlideStarted(InputAction.CallbackContext ctx) 
    {
        GameObject temp = GameManager.playerInputs.Detection();
        if(temp == this && temp !=null)
        {          
            _UpdateWheel = StartCoroutine(UpdateWheel());
        }
    }

    void OnFingerSlideEnded(InputAction.CallbackContext ctx)
    {
        if(_UpdateWheel == null) StopCoroutine(_UpdateWheel);
    }

    IEnumerator UpdateWheel()
    {
        while (true)
        {
            GameManager.Instance.GetSetWheelValue = (GameManager.playerInputs.FingerPosition.y - GameManager.playerInputs.SlideStartPos.y) * _multiplyFactor;
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
}
