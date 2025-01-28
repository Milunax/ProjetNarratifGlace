using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseNode : Node
{
    protected string nodeGuid;
    protected DialogueGraphView dialogueGraphView;
    protected DialogueEditorWindow editorWindow;
    protected LanguageType languageType = LanguageType.French;

    protected Vector2 defaultNodeSize = new Vector2(200, 250);

    public string NodeGuid { get => nodeGuid; set => nodeGuid = value; }

    public BaseNode() 
    {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("NodeStyleSheet");
        styleSheets.Add(styleSheet);
    }

    public void AddOutputPort(string name, Port.Capacity capacity = Port.Capacity.Single)
    {
        Port outputPort = GetPortInstance(Direction.Output, capacity);
        outputPort.portName = name;
        outputContainer.Add(outputPort);
    }

    public void AddInputPort(string name, Port.Capacity capacity = Port.Capacity.Multi)
    {
        Port inputPort = GetPortInstance(Direction.Input, capacity);
        inputPort.portName = name;
        inputContainer.Add(inputPort);
    }

    public Port GetPortInstance(Direction nodedirection, Port.Capacity capacity = Port.Capacity.Single)
    { 
        return InstantiatePort(Orientation.Horizontal, nodedirection, capacity, typeof(float));
    }

    protected void AddField<T, Y>(ref T _HolderField, ref Y _HolderValue, Action<Y> _UpdateValue) where T : BaseField<Y>, new()
    {
        _HolderField = new T { value = _HolderValue };
        // When the value from the drodown menu gets changed, save the new value and set it
        _HolderField.RegisterValueChangedCallback((value) =>
        {
            //_HolderValue = (Y)value.newValue;
            _UpdateValue(value.newValue);
        });
        _HolderField.SetValueWithoutNotify(_HolderValue);

        mainContainer.Add(_HolderField);
    }

    public virtual void LoadValueIntoField()
    {

    }
}
