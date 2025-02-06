using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if LITTLE_GRAPH
[LGNodeInfo("Trigger Event", "Little Dialogue/Trigger Event")]
public class TriggerEvent : LGNode
{
    public static event Action<string, bool> DialogueEvent;

    [ExposedProperty] public string eventTag;
    [ExposedProperty] public bool IsDialogueInteractive;

    protected override void ExecuteNode()
    {
        base.ExecuteNode();

        DialogueEvent?.Invoke(eventTag, IsDialogueInteractive);

        if (NodeConnections.Exists(connection => connection.OutputPort.NodeId == ID))
        {
            LGConnection connection =
                NodeConnections.Find(connection => connection.OutputPort.NodeId == ID);

            EmitFlow(connection.InputPort.NodeId);
        }
    }
}
#endif
