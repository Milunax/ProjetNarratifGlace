using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPassword : BaseKeypadText
{
    [SerializeField] private int _numberOfChar;

    public int numberOfChar => _numberOfChar;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GeneratePassword();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GeneratePassword()
    {
        string generatedPassword = "";

        for (int i  = 0; i < _numberOfChar; i++)
        {
            generatedPassword += Random.Range(0, _numberOfChar + 1);
        }

        _textComponent.text = generatedPassword;
    }
}
