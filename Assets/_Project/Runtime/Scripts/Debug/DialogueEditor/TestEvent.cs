using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Color _color;
    [SerializeField] private string _eventTag;

    private void OnEnable()
    {
        //DialogueEventSO.DialogueEvent += ColorChange;
    }

    private void OnDisable()
    {
        //DialogueEventSO.DialogueEvent -= ColorChange;
    }

    private void ColorChange(string tag)
    {
        if(tag == _eventTag)
        {
            _sr.color = _color;
        }
    }
}
