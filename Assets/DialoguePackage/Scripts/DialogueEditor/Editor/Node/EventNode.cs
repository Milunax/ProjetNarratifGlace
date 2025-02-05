using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class EventNode : BaseNode
{
    private DialogueEventSO dialogueEvent;
    private ObjectField eventField;

    private string eventTag;
    private TextField eventTagField;

    private bool eventBool = true;
    private Toggle eventBoolField;

    public DialogueEventSO DialogueEvent { get => dialogueEvent; set => dialogueEvent = value; }
    public string EventTag { get => eventTag; set => eventTag = value; }
    public bool EventBool { get => eventBool; set => eventBool = value; }

    public EventNode()
    {

    }

    public EventNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
    {
        editorWindow = _editorWindow;
        dialogueGraphView = _graphView;


        title = "Event";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Multi);
        AddOutputPort("Output", Port.Capacity.Single);

        eventField = new ObjectField()
        {
            objectType = typeof(DialogueEventSO),
            allowSceneObjects = false,
            value = dialogueEvent,
        };
        // Update value when value changed
        eventField.RegisterValueChangedCallback(value =>
        {
            dialogueEvent = eventField.value as DialogueEventSO;
        });
        eventField.SetValueWithoutNotify(dialogueEvent);
        mainContainer.Add(eventField);

        //Tag Label
        Label tagLabel = new Label("Event Tag");
        tagLabel.AddToClassList("label_text");
        tagLabel.AddToClassList("label");
        mainContainer.Add(tagLabel);

        //Tag field
        eventTagField = new TextField("");
        eventTagField.RegisterValueChangedCallback(value =>
        {
            eventTag = value.newValue;
        });
        eventTagField.AddToClassList("custom-text-field");
        eventTagField.SetValueWithoutNotify(eventTag);
        eventTagField.multiline = true;
        eventTagField.AddToClassList("TextBox");
        mainContainer.Add(eventTagField);

        //Bool Label
        Label boolLabel = new Label("Is dialogue interactive");
        tagLabel.AddToClassList("label_text");
        tagLabel.AddToClassList("label");
        mainContainer.Add(boolLabel);

        //Bool field
        AddField(ref eventBoolField, ref eventBool,
            newValue => eventBool = newValue);
    }

    public override void LoadValueIntoField()
    {
        eventField.SetValueWithoutNotify(dialogueEvent);
        eventTagField.SetValueWithoutNotify(eventTag);
        eventBoolField.SetValueWithoutNotify(eventBool);
    }
}
