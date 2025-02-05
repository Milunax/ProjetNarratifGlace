using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue Event")]
[System.Serializable]
public class DialogueEventSO : ScriptableObject
{
    public static event Action<string, bool> DialogueEvent;

    public virtual void RunEvent(string tag, bool isTrue)
    {
        DialogueEvent?.Invoke(tag, isTrue);
    }
}
