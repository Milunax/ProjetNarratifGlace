using LittleGraph.Editor;
using LittleGraph.Editor.Attributes;
using LittleGraph.Runtime.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

#if LITTLE_GRAPH
[LGCustomEditorNode(typeof(AntoineNode))]
public class AntoineEditorNode : LGEditorNode
{
   
}
#endif