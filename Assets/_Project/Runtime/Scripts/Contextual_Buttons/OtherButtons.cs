using System;
using UnityEngine;
using GMSpace;

public class OtherButtons : MonoBehaviour
{
    [SerializeField] private CONTEXTUAL_INPUT_INFO _keyInput;

    public Action OnClick;

    public CONTEXTUAL_INPUT_INFO Clicked()
    {
        GameManager.soundManager.PlayAudio("click sound", SoundManager.AUDIO_CATEGORY.NONE, 0.05f);
        return _keyInput;
    }
}
