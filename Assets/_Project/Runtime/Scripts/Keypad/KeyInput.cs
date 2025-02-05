using GMSpace;
using System;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    [SerializeField] private char _keyChar;

    public Action OnClick;

    public char Clicked()
    {
        GameManager.soundManager.PlayAudio("click sound", SoundManager.AUDIO_CATEGORY.NONE, 0.05f);

        return _keyChar;
    }
}
