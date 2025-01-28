using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class StartNode : BaseNode
{
    private string idStartNode = "ID_Start";
    public string IdStartNode { get => idStartNode; set => idStartNode = value; }
    
    private bool isMain = true;
    public bool IsMain { get => isMain; set => isMain = value; }

    private TextField textField;
    private Toggle toggleField;

    public StartNode()
    {

    }
    public StartNode(Vector2 position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
    {
        editorWindow = _editorWindow;
        dialogueGraphView = _graphView;

        title = "Start";
        SetPosition(new Rect(position, defaultNodeSize));
        NodeGuid = Guid.NewGuid().ToString();

        AddOutputPort("Output", Port.Capacity.Single);
        AddField(ref toggleField, ref isMain, 
            newValue => isMain = newValue);
        AddField(ref textField, ref idStartNode, 
            newValue => IdStartNode = newValue);
        textField.SetValueWithoutNotify("test");
        //Tells the system we changed the node and to update it.
        RefreshExpandedState();
        RefreshPorts();
    }
    public override void LoadValueIntoField()
    {
        textField.SetValueWithoutNotify(idStartNode);
        toggleField.SetValueWithoutNotify(isMain);
    }
}
