using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNode : BaseNode
{
    // Parameters
    private string Key;
    private List<AudioClip> audioClips = new List<AudioClip>();
    private string nameDialogue = "";
    private string tmpString = "";
    private Sprite faceImage;
    private DialogueSpriteType facingDirection;

    public (Port, int) MyPorts;

    private List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();
    // Get Sets
    public string Text { get => Key; set => Key = value; }
    //public List<AudioClip> AudioClips { get => audioClips; set => audioClips = value; }
    public string Name { get => nameDialogue; set => nameDialogue = value; }
    public Sprite FaceImage { get => faceImage; set => faceImage = value; }
    public DialogueSpriteType FacingDirection { get => facingDirection; set => facingDirection = value; }
    public List<DialogueNodePort> DialogueNodePorts { get => dialogueNodePorts; set => dialogueNodePorts = value; }

    // Fields
    private TextField text_Field;
    private ObjectField audioClips_Field;
    private ObjectField image_Field;
    private TextField name_Field;
    private EnumField facingDirection_field;

    public DialogueNode()
    {

    }

    public DialogueNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
    {
        editorWindow = _editorWindow;
        dialogueGraphView = _graphView;

        title = "Dialogue";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Multi);
        //AddOutputPort("Output", Port.Capacity.Single);

        // get the values that correspond to current language (Change when localisation thingy is done)
/*        foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
        {
            Key.Add(new LanguageGeneric<string>
            {
                languageType = language,
                LanguageGenericType = "",
            });

            audioClips.Add(new LanguageGeneric<AudioClip>
            {
                languageType = language,
                LanguageGenericType = null,
            });
        }*/

        // Face Image Field
        image_Field = new ObjectField
        {
            objectType = typeof(Sprite),
            // Cannot link with instanciated objects in scene, only assets in the project files
            allowSceneObjects = false,
            value = faceImage,
        };
        image_Field.RegisterValueChangedCallback(value =>
        {
            faceImage = value.newValue as Sprite;
            //faceImage = image_Field.value;
        });
        mainContainer.Add(image_Field);

        // Face Image Enum
        facingDirection_field = new EnumField()
        {
            value = facingDirection,

        };
        facingDirection_field.Init(FacingDirection);
        facingDirection_field.RegisterValueChangedCallback((value) =>
        {
            facingDirection = (DialogueSpriteType)value.newValue;
        });
        mainContainer.Add(facingDirection_field);
        
        // Name Field
        Label label_name = new Label("Name");
        label_name.AddToClassList("label_name");
        label_name.AddToClassList("label");
        mainContainer.Add(label_name);

        name_Field = new TextField("");
        name_Field.RegisterValueChangedCallback(value =>
        {
            nameDialogue = value.newValue;
        });
        name_Field.SetValueWithoutNotify(nameDialogue);
        //name_Field.AddToClassList("Text");
        mainContainer.Add(name_Field);

        // Text Field
        Label label_text = new Label("Key Text");
        label_text.AddToClassList("label_text");
        label_text.AddToClassList("label");
        mainContainer.Add(label_text);

        text_Field = new TextField("");
        text_Field.RegisterValueChangedCallback(value =>
        {
            Key  = value.newValue;

        });
        text_Field.AddToClassList("custom-text-field");
        text_Field.SetValueWithoutNotify(Key);
        text_Field.multiline = true;
        text_Field.AddToClassList("TextBox");
        mainContainer.Add(text_Field);


        Button button = new Button()
        {
            text = "Add Choice"
        };
        button.clicked += () =>
        {
            AddChoicePort(this);
        };
        titleButtonContainer.Add(button);
    }

    public override void LoadValueIntoField()
    {
        //TODO Update these two when you know where you get the text
        text_Field.SetValueWithoutNotify(Key);
        //audioClips_Field.SetValueWithoutNotify(null);


        image_Field.SetValueWithoutNotify(faceImage);
        facingDirection_field.SetValueWithoutNotify(facingDirection);
        name_Field.SetValueWithoutNotify(nameDialogue);
    }

    public Port AddChoicePort(BaseNode _basenode, DialogueNodePort _dialogueNodePort = null)
    {
        Port port = GetPortInstance(Direction.Output);
        // check how many outputs already exist
        int outputPortCount = _basenode.outputContainer.Query("connector").ToList().Count();
        string outputPortName = $"Choice {outputPortCount + 1}";

        DialogueNodePort dialogueNodePort = new DialogueNodePort();

        if (_dialogueNodePort != null)
        {
            dialogueNodePort.InputGuid = _dialogueNodePort.InputGuid;
            dialogueNodePort.OutputGuid = _dialogueNodePort.OutputGuid;
            dialogueNodePort.Key = _dialogueNodePort.Key;
        }

        // Text for the port
        dialogueNodePort.textField = new TextField();
        dialogueNodePort.textField.AddToClassList("custom-text-field");

        // Register value changed callback
        dialogueNodePort.textField.RegisterValueChangedCallback(value =>
        {
            tmpString = value.newValue; // Assuming TextValue stores the text for this port
            dialogueNodePort.Key = value.newValue;
        });
        // Set the initial value
        //dialogueNodePort.textField.SetValueWithoutNotify(tmpString);
        //                                                  CONDITION                   IF TRUE             IF FALSE
        dialogueNodePort.textField.SetValueWithoutNotify(_dialogueNodePort != null ? _dialogueNodePort.Key:tmpString);

        // Add the text field to the port
        port.contentContainer.Add(dialogueNodePort.textField);

        // Delete button
        Button deleteButton = new Button(() => DeletePort(_basenode, port))
        {
            text = "X",
        };
        port.contentContainer.Add(deleteButton);
        dialogueNodePort.MyPort = port;
        port.portName = "";
        dialogueNodePorts.Add(dialogueNodePort);

        _basenode.outputContainer.Add(port);
            
        //Refresh
        _basenode.RefreshPorts();
        _basenode.RefreshExpandedState();
        return port;
    }
    private void DeletePort(BaseNode _node, Port _port)
    {
        // find the port
        DialogueNodePort tmp = dialogueNodePorts.Find(port => port.MyPort == _port);
        // remove it from the list
        dialogueNodePorts.Remove(tmp);

        // find connections to port to be removed
        IEnumerable<Edge> portEdge = dialogueGraphView.edges.ToList().Where(edge => edge.output == _port);
        // remove all the connexions of the deleted node
        if (portEdge.Any())
        {
            Edge edge = portEdge.First();
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);
            dialogueGraphView.RemoveElement(edge);
        };
        _node.outputContainer.Remove(_port);
        //Refresh
        _node.RefreshPorts();
        _node.RefreshExpandedState();
    }
}


/*        // Audio Clips Field
        audioClips_Field = new ObjectField()
        {
            objectType = typeof(AudioClip),
            allowSceneObjects = false,
            value = audioClips.Find(audioClip => audioClip.languageType == editorWindow.LanguageType).LanguageGenericType,

        };
        audioClips_Field.RegisterValueChangedCallback(value =>
        {
            audioClips.Find(audioClip => audioClip) = value.newValue as AudioClip;
        });
        audioClips_Field.SetValueWithoutNotify(audioClips.Find(audioClip =>  audioClip.languageType == editorWindow.LanguageType).LanguageGenericType);
        mainContainer.Add(audioClips_Field);

        // Audio Clips Field
        audioClips_Field = new ObjectField()
        {
            objectType = typeof(AudioClip), // Specifies the field accepts AudioClip objects
            allowSceneObjects = false,     // Ensures only project assets, not scene objects, can be assigned
        };

        // Register callback to handle value changes
        audioClips_Field.RegisterValueChangedCallback(evt =>
        {
            // Store the selected AudioClip
            audioClips.Find() = evt.newValue as AudioClip;
        });

        // Add the ObjectField to the main container
        mainContainer.Add(audioClips_Field);*/