using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    [SerializeField] private char _keyChar;

    public Action<TextMeshProUGUI> OnClick;

    // Start is called before the first frame update
    void Start()
    {
        OnClick += Clicked;
    }

    private void OnDisable()
    {
        OnClick -= Clicked;
    }

    private void Clicked(TextMeshProUGUI TextToWriteIn)
    {
        //Debug.Log(name);
        TextToWriteIn.text += _keyChar;
    }
}
