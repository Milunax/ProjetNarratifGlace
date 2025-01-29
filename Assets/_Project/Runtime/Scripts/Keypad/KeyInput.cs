using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    [SerializeField] private char _keyChar;

    public Action OnClick;

    // Start is called before the first frame update
    void Start()
    {
        OnClick += Clicked;
    }

    private void OnDisable()
    {
        OnClick -= Clicked;
    }

    private void Clicked()
    {
        Debug.Log(_keyChar);
    }
}
