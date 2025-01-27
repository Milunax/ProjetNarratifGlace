using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class EndNode : BaseNode
{
    private EndNodeTypes endNodeType = EndNodeTypes.End;
    private EnumField enumField;

    public EndNodeTypes EndNodeType { get => endNodeType; set => endNodeType = value; }
    public EndNode() { }
    public EndNode(Vector2 position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
    {
        editorWindow = _editorWindow;
        dialogueGraphView = _graphView;

        title = "End";
        SetPosition(new Rect(position, defaultNodeSize));
        NodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Multi);

        // Dropdown menu (EndType)
        enumField = new EnumField() { value = endNodeType };
        enumField.Init(endNodeType);
        // When the value from the drodown menu gets changed, save the new value and set it
        enumField.RegisterValueChangedCallback((value) =>
        {
            endNodeType = (EndNodeTypes)value.newValue;
        });
        enumField.SetValueWithoutNotify(endNodeType);

        mainContainer.Add(enumField);
        //Tells the system we changed the node and to update it.
        RefreshExpandedState();
        RefreshPorts();

    }
    public override void LoadValueIntoField()
    {
        enumField.SetValueWithoutNotify(endNodeType);
    }
}

