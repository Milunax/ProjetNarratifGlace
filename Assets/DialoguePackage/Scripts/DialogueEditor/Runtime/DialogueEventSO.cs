using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue Event")]
[System.Serializable]
public class DialogueEventSO : ScriptableObject
{
    public static event Action<string> DialogueEvent;

    public virtual void RunEvent(string tag)
    {
        DialogueEvent?.Invoke(tag);
    }
}
