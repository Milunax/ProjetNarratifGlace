using GMSpace;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class WheelInput : MonoBehaviour
{
    public static event Action<bool> OnKeyPressed;
    private bool _wasInput;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private AudioClip _audioWheel;
    [SerializeField] private float _multiplyFactor = 0.2f;
    private AudioSource _audioSourceWheel;
    private Coroutine _coroutine;

    private void Start()
    {
        GameManager.playerInputs.primaryTouch.action.started += CheckButtonPressed;
        GameManager.playerInputs.primaryTouch.action.canceled += CheckButtonUnpressed;
    }

    private void OnDisable()
    {
        GameManager.playerInputs.primaryTouch.action.started -= CheckButtonPressed;
        GameManager.playerInputs.primaryTouch.action.canceled -= CheckButtonUnpressed;
    }

    private void CheckButtonPressed(CallbackContext ctx)
    {
        var goDetected = GameManager.playerInputs.Detection();

        if (goDetected == gameObject)
        {
            _wasInput = true;
            OnKeyPressed?.Invoke(true);
            _audioSourceWheel = GameManager.soundManager.PlayAudioLoop(_audioWheel, SoundManager.AUDIO_CATEGORY.FOLEY);
            _coroutine = StartCoroutine(Effects());
        }
    }
    private void CheckButtonUnpressed(CallbackContext ctx)
    {
        if (_wasInput)
        {
            _wasInput = false;
            OnKeyPressed?.Invoke(false);
            GameManager.soundManager.StopAudioLoop(ref _audioSourceWheel);
            StopCoroutine(_coroutine);
        }
    }

    private IEnumerator Effects()
    {
        float startValue = GameManager.Instance.GetSetWheelValue;
        while (true)
        {
            GameManager.Instance.GetSetWheelValue = startValue + (GameManager.playerInputs.GetSlideDeltaV.y * _multiplyFactor);

            if (GameManager.Instance.GetSetWheelValue <= GameManager.Instance.GetWheelMin || GameManager.Instance.GetSetWheelValue >= GameManager.Instance.GetWheelMax)
            {
                GameManager.soundManager.ChangeAudioLoopVolume(_audioSourceWheel, 0);
            }
            else
            {
                GameManager.soundManager.ChangeAudioLoopVolume(_audioSourceWheel, Mathf.Abs(GameManager.playerInputs.GetFingerDelta.y / 10));
                _gameObject.transform.Rotate(new Vector3(0, GameManager.playerInputs.GetFingerDelta.y, 0));
            }
            
            yield return new WaitForFixedUpdate();
        }
    }
}
