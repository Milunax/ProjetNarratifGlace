using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    private string styleSheetsName = "GraphViewStyleSheet";
    private DialogueEditorWindow dialogueEditorWindow;
    private NodeSearchWindow searchWindow;
    public DialogueGraphView(DialogueEditorWindow _editorWindow) 
    {
        dialogueEditorWindow = _editorWindow;

        StyleSheet tmpStyleSheet = Resources.Load<StyleSheet>(styleSheetsName);
        styleSheets.Add(tmpStyleSheet);

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());
        GridBackground grid = new GridBackground();
        Insert(0, grid);

        grid.StretchToParentSize();

        AddSearchWindow();
    }
    public StartNode CreateStartNode(Vector2 _pos)
    {
        StartNode tmp = new StartNode(_pos, dialogueEditorWindow, this);
        return tmp;
    }

    public DialogueNode CreateDialogueNode(Vector2 _pos)
    {
        DialogueNode tmp = new DialogueNode(_pos, dialogueEditorWindow, this);
        return tmp;
    }

    public EventNode CreateEventNode(Vector2 _pos)
    {
        EventNode tmp = new EventNode(_pos, dialogueEditorWindow, this);
        return tmp;
    }

    public EndNode CreateEndNode(Vector2 _pos)
    {
        EndNode tmp = new EndNode(_pos, dialogueEditorWindow, this);
        return tmp;
    }

    private void AddSearchWindow()
    {
        searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        searchWindow.Configure(dialogueEditorWindow, this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();
        Port startPortView = startPort;

        ports.ForEach((port) =>
        {
            // We make a port
            Port portView = port;
            // check that: we're not connecting to ourselves, nor to any ports on our node and finally not to a port that is the same direction as us (left cannot connect to left)
            if(startPortView != portView && startPortView.node != portView.node && startPortView.direction != port.direction)
            {
                compatiblePorts.Add(port);
            }
        });
        return compatiblePorts;
    }
}
