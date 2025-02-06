using GMSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimonPatternBehaviour : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Image _redSimon;
    [SerializeField] private Image _yellowSimon;
    [SerializeField] private Image _greenSimon;
    [SerializeField] private Image _blueSimon;
    [SerializeField] private Image _backGround;

    private List<SIMON_PAD_INFO> _pressedSuite;
    private List<SIMON_PAD_INFO> _askedSuite;

    private int _suiteSize;
    private Coroutine _coroutine;

    public static event Action<bool> OnSimonEnd;

    private void OnEnable()
    {
        SimonPad.OnKeyPressed += AddInput;
        ContextualButtons.OnKeyPressed += BackSpaceSelection;
    }
    private void OnDisable()
    {
        SimonPad.OnKeyPressed -= AddInput;
        ContextualButtons.OnKeyPressed -= BackSpaceSelection;
    }

    public static void SimonSound(SIMON_PAD_INFO input)
    {
        float pitch;
        switch (input)
        {
            case SIMON_PAD_INFO.RED:
                pitch = -0.15f;
                break;
            case SIMON_PAD_INFO.YELLOW:
                pitch = -0.05f;
                break;
            case SIMON_PAD_INFO.GREEN:
                pitch = 0.05f;
                break;
            case SIMON_PAD_INFO.BLUE:
                pitch = 0.15f;
                break;

            default:
                pitch = 0f;
                break;
        }

        GameManager.soundManager.PlayAudioWithPitch("simon a pitch", pitch);
    }

    public void Opening()
    {
        _redSimon.gameObject.SetActive(true);
        _yellowSimon.gameObject.SetActive(true);
        _greenSimon.gameObject.SetActive(true);
        _blueSimon.gameObject.SetActive(true);
        _backGround.gameObject.SetActive(true);

        _redSimon.color = Color.white;
        _greenSimon.color = Color.white;
        _yellowSimon.color = Color.white;
        _blueSimon.color = Color.white;
    }
    public void Closing()
    {
        _redSimon.gameObject.SetActive(false);
        _yellowSimon.gameObject.SetActive(false);
        _greenSimon.gameObject.SetActive(false);
        _blueSimon.gameObject.SetActive(false);
        _backGround.gameObject.SetActive(false);
    }

    public void CallSimonSuite(int nb)
    {
        _suiteSize = nb;

        _pressedSuite = new List<SIMON_PAD_INFO>();
        _askedSuite = new List<SIMON_PAD_INFO>();

        for (int i = 0; i < nb; i++)
        {
            _askedSuite.Add((SIMON_PAD_INFO)UnityEngine.Random.Range(1, 5));
        }

        _coroutine = StartCoroutine(ShowPattern());
    }
    private IEnumerator ShowPattern()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            foreach (SIMON_PAD_INFO item in _askedSuite)
            {
                SimonSound(item);
                switch (item)
                {
                    case SIMON_PAD_INFO.GREEN:
                        _greenSimon.color = Color.green;
                        yield return new WaitForSeconds(0.2f);
                        _greenSimon.color = Color.white;
                        break;

                    case SIMON_PAD_INFO.BLUE:
                        _blueSimon.color = Color.blue;
                        yield return new WaitForSeconds(0.2f);
                        _blueSimon.color = Color.white;
                        break;

                    case SIMON_PAD_INFO.YELLOW:
                        _yellowSimon.color = Color.yellow;
                        yield return new WaitForSeconds(0.2f);
                        _yellowSimon.color = Color.white;
                        break;

                    case SIMON_PAD_INFO.RED:
                        _redSimon.color = Color.red;
                        yield return new WaitForSeconds(0.2f);
                        _redSimon.color = Color.white;
                        break;
                }

                yield return new WaitForSeconds(0.3f);
            }

            yield return new WaitForSeconds(4);
        }
    }

    public void AddInput(SIMON_PAD_INFO info)
    {
        SimonSound(info);
        _pressedSuite.Add(info);

        if (_pressedSuite.Count >= _askedSuite.Count)
        {
            for (int i = 0; i < _askedSuite.Count; i++)
            {
                if (_pressedSuite[i] == _askedSuite[i])
                {
                    if (_coroutine != null) StopCoroutine(_coroutine);
                    GameManager.soundManager.PlayAudio("denied", SoundManager.AUDIO_CATEGORY.SFX, 0.05f);
                    OnSimonEnd?.Invoke(false);
                    CallSimonSuite(_suiteSize);
                    break;
                }
            }

            if (_coroutine != null) StopCoroutine(_coroutine);
            GameManager.soundManager.PlayAudio("task done_2", SoundManager.AUDIO_CATEGORY.SFX);
            OnSimonEnd?.Invoke(true);
        }
    }
    public void BackSpaceSelection(CONTEXTUAL_INPUT_INFO info)
    {
        if (info == CONTEXTUAL_INPUT_INFO.BACKSPACE)
        {
            GameManager.soundManager.PlayAudio("denied", SoundManager.AUDIO_CATEGORY.SFX, 0.05f);
            OnSimonEnd?.Invoke(false);
            CallSimonSuite(_suiteSize);
        }
    }
}
