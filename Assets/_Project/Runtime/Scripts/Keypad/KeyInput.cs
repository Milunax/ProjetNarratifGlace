using GMSpace;
using System;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    [SerializeField] private char _keyChar;

    public Action OnClick;

    public char Clicked()
    {

        return _keyChar;
    }
}
