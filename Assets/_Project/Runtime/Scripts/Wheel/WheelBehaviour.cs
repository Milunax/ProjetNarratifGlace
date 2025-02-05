using GMSpace;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class WheelBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _waveCurrent;
    [SerializeField] private GameObject _waveTarget;
    private Image _imageWaveCurrent;
    private Image _imageWaveTarget;

    [SerializeField] private AudioClip _audioWheel;
    private AudioSource _audioSourceWheel;

    [SerializeField] private AudioClip _audioNoise;
    private AudioSource _audioSourceNoise;
    private AudioSource _audioSourceFile;

    [Header("Parameters")]
    [SerializeField] private float _minWheelVal = 0.0f;
    [SerializeField] private float _maxWheelVal = 100.0f;
    [SerializeField] private float _margin = 10f;
    [SerializeField] private float _multiplyFactor = 1f;

    private Coroutine _UpdateWheel;
    private bool _isActive;

    private float _waveCurrentValue;
    private float _waveTargetValue;
    public float GetWaveCurrent {  get => _waveCurrentValue; }
    public float GetWaveTarget { get => _waveTargetValue; }

    private void OnEnable()
    {
        WheelInput.OnKeyPressed += OnFingerUpdated;
    }
    private void OnDisable()
    {
        WheelInput.OnKeyPressed -= OnFingerUpdated;
    }

    void Start()
    {
        GameManager.Instance.SetWheelMinMax(_minWheelVal, _maxWheelVal);

        _imageWaveCurrent = _waveCurrent.GetComponent<Image>();
        _imageWaveTarget = _waveTarget.GetComponent<Image>();
    }

    public void Opening()
    {
        _waveCurrent.SetActive(true);
        _waveTarget.SetActive(true);
    }
    public void Closing()
    {
        _waveCurrent.SetActive(false);
        _waveTarget.SetActive(false);
    }

    private void SetValues()
    {
        _waveTargetValue = Random.Range(_minWheelVal, _maxWheelVal);
        _imageWaveTarget.material.SetFloat("_Wave1Frequency", _waveTargetValue);

        do
        {
            _waveTargetValue = Random.Range(_minWheelVal, _maxWheelVal);
        } while (_waveCurrentValue >= _waveTargetValue - _margin && _waveCurrentValue <= _waveTargetValue + _margin);
        GameManager.Instance.GetSetWheelValue = _waveCurrentValue;
        _imageWaveCurrent.material.SetFloat("_Wave1Frequency", _waveCurrentValue);

    }

    void OnFingerUpdated(bool start)
    {
        if (start)
        {
            GameObject temp = GameManager.playerInputs.Detection();
            if (temp != null && temp == gameObject)
            {
                _audioSourceWheel = GameManager.soundManager.PlayAudioLoop(_audioWheel, SoundManager.AUDIO_CATEGORY.FOLEY);

                _UpdateWheel = StartCoroutine(UpdateWheel());
            }
        }
        else
        {
            if (_UpdateWheel != null)
            {
                StopCoroutine(_UpdateWheel);

                GameManager.soundManager.StopAudioLoop(ref _audioSourceWheel);
            }
        }
    }

    IEnumerator UpdateWheel()
    {
        float startValue = GameManager.Instance.GetSetWheelValue;
        while (true)
        {
            GameManager.Instance.GetSetWheelValue = startValue + (GameManager.playerInputs.GetSlideDeltaV.y * _multiplyFactor);

            _imageWaveCurrent.material.SetFloat("_Wave1Frequency", GameManager.Instance.GetSetWheelValue/_maxWheelVal * 10);
            //_imageWaveTarget.material.SetFloat("_Wave1Frequency", GameManager.Instance.GetSetWheelValue/_maxWheelVal * 10);

            //print(_canvaRendererWave.GetMaterial().GetFloat("_Wave1Frequency"));
            //if (_saveWaveTargetValue == GameManager.Instance.GetSetWheelValue)
            //{
            //    Debug.Log("Wave game Win");
            //}
            yield return new WaitForFixedUpdate();
        }
    }
}
