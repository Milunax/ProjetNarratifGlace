using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue")]
[System.Serializable]
public class DialogueContainerSO : ScriptableObject
{
    public List<NodeLinkData> nodeLinkDatas = new List<NodeLinkData>();

    public List<DialogueNodeData> dialogueNodeDatas = new List<DialogueNodeData>();
    public List<EndNodeData> endNodeDatas = new List<EndNodeData>();
    public List<StartNodeData> startNodeDatas = new List<StartNodeData>();
    public List<EventNodeData> eventNodeDatas = new List<EventNodeData>();

    public List<BaseNodeData> AllNodes
    {
        get
        {
            List<BaseNodeData> tmp = new List<BaseNodeData>();
            tmp.AddRange(dialogueNodeDatas);
            tmp.AddRange(endNodeDatas);
            tmp.AddRange(startNodeDatas);
            tmp.AddRange(eventNodeDatas);
            return tmp;
        }
    }
}

#region data

[System.Serializable]
public class NodeLinkData
{
    public string baseNodeGuid;
    public string targetNodeGuid;

}
[System.Serializable]
public class BaseNodeData
{
    public string nodeguid;
    public Vector2 position;
}


[System.Serializable]
public class DialogueNodeData : BaseNodeData
{
    public List<DialogueNodePort> dialogueNodePorts;
    public Sprite sprite;
    public DialogueSpriteType dialoguefaceimagetype;
    // public AudioClip audioclips; Implementing this later for now i want my code to work
    public string name;
    public string key;
}

[System.Serializable]
public class EndNodeData : BaseNodeData 
{
    public EndNodeTypes endNodeTypes;
}

[System.Serializable]
public class StartNodeData :BaseNodeData
{
    public string node_id;
    public bool mainStartNode;
}

[System.Serializable]
public class EventNodeData : BaseNodeData
{
    public string tag;
    public bool isInteractive;
    public DialogueEventSO DialogueEventSo;
}

#endregion data

[System.Serializable]
public class DialogueNodePort
{
    public string InputGuid;
    public string OutputGuid;
    public UnityEditor.Experimental.GraphView.Port MyPort;
    public PortParameters MyPortParams;
    public TextField textField;
    public string Key;

    public int ID_Port;
}

public class PortParameters
{
    public PortDirection portDirection;
    public PortOrientation portOrientation;
    public PortCapacity portCapacity;
};

public enum PortDirection
{
    Left,
    Right,
}
public enum PortOrientation
{
    Vertical,
    Horizontal,
}
public enum PortCapacity
{
    Single,
    Multi,
}