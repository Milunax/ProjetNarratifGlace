using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextInput : BaseKeypadText
{
    protected override void Start()
    {
        base.Start();

        textComponent.text = "";
    }
}
