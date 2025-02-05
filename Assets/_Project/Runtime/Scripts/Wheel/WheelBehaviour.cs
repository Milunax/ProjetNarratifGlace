using GMSpace;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class WheelBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _waveImage;
    [SerializeField] GameObject _waveTarget;
    [SerializeField] AudioClip _audio;
    AudioSource _audioSource;
    CanvasRenderer _canvaRendererWave;
    CanvasRenderer _canvaRendererWaveTarget;

    [Header("Parameters")]
    [SerializeField] float _minWheelVal = 0.0f;
    [SerializeField] float _maxWheelVal = 100.0f;
    [SerializeField] float _multiplyFactor = 1f;
    [SerializeField] TMP_Text _scrolltext;

    private Coroutine _UpdateWheel;
    private bool _isActive;

    private float _saveStartValue;
    private float _saveWaveTargetValue;
    public float GetSaveStartValue {  get => _saveStartValue;}


    void Start()
    {
        GameManager.playerInputs.primaryTouch.action.started += OnFingerSlideStarted;
        GameManager.playerInputs.primaryTouch.action.canceled += OnFingerSlideEnded;

        GameManager.Instance.SetWheelMinMax(_minWheelVal, _maxWheelVal);
        _isActive = GameManager.Instance.GetSetWaveValidity;
        _canvaRendererWave = _waveImage.GetComponent<CanvasRenderer>();
        _canvaRendererWaveTarget = _waveTarget.GetComponent<CanvasRenderer>();
        //_waveImage.SetActive(false);
        StartCoroutine(GetWaveMaterial());
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
            _isActive = true;
            _waveImage.SetActive(true);
            _audioSource = GameManager.soundManager.PlayAudioLoop(_audio, SoundManager.AUDIO_CATEGORY.FOLEY);
            _UpdateWheel = StartCoroutine(UpdateWheel());
        }
    }

    void OnFingerSlideEnded(InputAction.CallbackContext ctx)
    {
        if (_UpdateWheel != null)
        {
            _isActive = false;
            StopCoroutine(_UpdateWheel);
            _waveImage.SetActive(false);
            GameManager.soundManager.StopAudioLoop(ref _audioSource);
        }
    }

    IEnumerator UpdateWheel()
    {
        float startValue = GameManager.Instance.GetSetWheelValue;
        while (true)
        {
            _scrolltext.text = GameManager.Instance.GetSetWheelValue.ToString();
            GameManager.Instance.GetSetWheelValue = startValue + (GameManager.playerInputs.GetSlideDeltaV.y * _multiplyFactor);
            _canvaRendererWave.GetComponent<Image>().material.SetFloat("_Wave1Frequency", GameManager.Instance.GetSetWheelValue/_maxWheelVal * 10);
            _canvaRendererWaveTarget.GetComponent<Image>().material.SetFloat("_Wave1Frequency", GameManager.Instance.GetSetWheelValue/_maxWheelVal * 10);
            //print(_canvaRendererWave.GetMaterial().GetFloat("_Wave1Frequency"));
            if (_saveWaveTargetValue == GameManager.Instance.GetSetWheelValue)
            {
                Debug.Log("Wave game Win");
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator GetWaveMaterial()
    {
        do
        {
            if (_canvaRendererWave.GetComponent<Image>().material != null && _canvaRendererWaveTarget.GetComponent<Image>().material != null)
            {
                //Debug.Log("not null");
                _saveStartValue = _canvaRendererWave.GetComponent<Image>().material.GetFloat("_Wave1Frequency");
                _saveWaveTargetValue = _canvaRendererWaveTarget.GetComponent<Image>().material.GetFloat("_Wave1Frequency");
            }

            yield return new WaitForFixedUpdate();
        } while (_canvaRendererWave.GetMaterial() == null);
    }
}
