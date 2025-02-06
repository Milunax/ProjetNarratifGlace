using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if LITTLE_GRAPH
[LGNodeInfo("Antoine Node", "Antoine System/Antoine Node")]
public class AntoineNode : LGNode
{
    [ExposedProperty] public string AntoineDebug;

    protected override void ExecuteNode()
    {
        base.ExecuteNode();

        //....Le truc du node
        Debug.Log(AntoineDebug);

        if (NodeConnections.Exists(connection => connection.OutputPort.NodeId == ID))
        {
            LGConnection connection =
                NodeConnections.Find(connection => connection.OutputPort.NodeId == ID);

            EmitFlow(connection.InputPort.NodeId);
        }
    }
}
#endif
