using GMSpace;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using LocalizationPackage;

public class WheelBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _waveCurrent;
    [SerializeField] private GameObject _waveTarget;
    private Image _imageWaveCurrent;
    private Image _imageWaveTarget;

    [SerializeField] private AudioClip _audioNoise;
    private AudioSource _audioSourceNoise;
    private AudioClip _audioFile;
    private AudioSource _audioSourceFile;

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _transcription;
    private List<AUDIO_TRANSCRIPTION> _transcriptionList = new List<AUDIO_TRANSCRIPTION>();
    private float _clipDuration;

    [Header("Parameters")]
    [SerializeField] private float _minWheelVal = 10.0f;
    [SerializeField] private float _maxWheelVal = 100.0f;
    [SerializeField] private float _margin = 4.5f;

    private Coroutine _updateWheel;
    private Coroutine _updateTimer;
    private bool _isPaused = false;

    private float _waveTargetValue;
    public float GetWaveCurrent {  get => GameManager.Instance.GetSetWheelValue; }
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
        _title.gameObject.SetActive(true);
        _transcription.gameObject.SetActive(true);
        _waveCurrent.SetActive(true);
        _waveTarget.SetActive(true);
    }
    public void Closing()
    {
        _title.gameObject.SetActive(false);
        _transcription.gameObject.SetActive(false);
        _waveCurrent.SetActive(false);
        _waveTarget.SetActive(false);

        GameManager.soundManager.StopAudioLoop(ref _audioSourceNoise);
        GameManager.soundManager.StopAudioLoop(ref _audioSourceFile);
    }

    public void SetTextValues(float textSize)
    {
        _title.fontSize = textSize;
        _transcription.fontSize = textSize;
    }
    public void SetValues(string title, List<AUDIO_TRANSCRIPTION> transcrition, AudioClip audio = null)
    {
        _transcriptionList = transcrition;
        _audioFile = audio;

        _title.text = LocalizationManager.Instance.UniGetText("FileExplorer_Translation", title);
        _transcription.text = LocalizationManager.Instance.UniGetText("FileExplorer_Translation", "Audio_error");

        if (audio != null) _clipDuration = audio.length;
        else _clipDuration = 0;

        _isPaused = false;

        _waveTargetValue = Random.Range(_minWheelVal, _maxWheelVal);
        _imageWaveTarget.material.SetFloat("_Wave1Frequency", _waveTargetValue / _maxWheelVal * 10);

        do
        {
            GameManager.Instance.GetSetWheelValue = Random.Range(_minWheelVal, _maxWheelVal);
        } while (GameManager.Instance.GetSetWheelValue >= _waveTargetValue - _margin && GameManager.Instance.GetSetWheelValue <= _waveTargetValue + _margin);

        _imageWaveCurrent.material.SetFloat("_Wave1Frequency", GameManager.Instance.GetSetWheelValue / _maxWheelVal * 10);

        GameManager.soundManager.StopAudioLoop(ref _audioSourceNoise);
        _audioSourceNoise = GameManager.soundManager.PlayAudioLoop(_audioNoise, SoundManager.AUDIO_CATEGORY.SFX);

        GameManager.soundManager.StopAudioLoop(ref _audioSourceFile);

        GameManager.Instance.GetSetWaveValidity = false;

        Debug.Log("Init");
    }

    void OnFingerUpdated(bool start)
    {
        if (start)
        {
            _updateWheel = StartCoroutine(UpdateWheel());
            Debug.Log("Coroutine");
        }
        else
        {
            if (_updateWheel != null)
            {
                StopCoroutine(_updateWheel);
            }
        }
    }

    private IEnumerator UpdateWheel()
    {
        while (true)
        {
            _imageWaveCurrent.material.SetFloat("_Wave1Frequency", GameManager.Instance.GetSetWheelValue / _maxWheelVal * 10);

            if (GameManager.Instance.GetSetWheelValue >= _waveTargetValue - _margin && GameManager.Instance.GetSetWheelValue <= _waveTargetValue + _margin)
            {
                if (GameManager.Instance.GetSetWaveValidity == false)
                {
                    GameManager.soundManager.PauseAudioLoop(_audioSourceNoise, true);
                    _audioSourceFile = GameManager.soundManager.PlayAudioLoop(_audioFile, SoundManager.AUDIO_CATEGORY.VOICES);
                    GameManager.Instance.GetSetWaveValidity = true;
                    _updateTimer = StartCoroutine(UpdateTimer());
                }
            }
            else if (GameManager.Instance.GetSetWaveValidity == true)
            {
                GameManager.soundManager.PauseAudioLoop(_audioSourceNoise, false);
                GameManager.soundManager.StopAudioLoop(ref _audioSourceFile);
                GameManager.Instance.GetSetWaveValidity = false;
                _transcription.text = LocalizationManager.Instance.UniGetText("FileExplorer_Translation", "Audio_error");
                StopCoroutine(_updateTimer);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator UpdateTimer()
    {
        float timer = 0;
        while (true)
        {
            if (_clipDuration > 0)
            {
                for (int i = 0; i < _transcriptionList.Count; i++)
                {
                    if (i + 1 >= _transcriptionList.Count || _transcriptionList[i + 1].timeCode > timer)
                    {
                        _transcription.text = LocalizationManager.Instance.UniGetText("FileExplorer_Translation", _transcriptionList[i].transcription);
                        break;
                    }
                }

                timer += Time.fixedDeltaTime;
                if (timer >= _clipDuration) timer -= _clipDuration;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
