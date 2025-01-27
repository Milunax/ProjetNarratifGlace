using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueSaveAndLoad
{
    // Whenever you call for the variable, gets the updated list of edges from the graph
    private List<Edge> edges => graphView.edges.ToList();

    // Whenever you call for this variable, gets the updated list of nodes from the graph, nodes that are BaseNode and then put into a List<BaseNode>
    private List<BaseNode> nodes => graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
    private DialogueGraphView graphView;
    public DialogueSaveAndLoad(DialogueGraphView _graphview)
    {
        graphView = _graphview;
    }

    public void Save(DialogueContainerSO _dialogueContainerSO)
    {
        SaveEdges(_dialogueContainerSO);
        SaveNodes(_dialogueContainerSO);

        // When saving a SO in runtime, you need to mark it as dirty
        EditorUtility.SetDirty(_dialogueContainerSO);
        // "Please save what has been changed", here _dialogueContainerSO
        AssetDatabase.SaveAssets();
    }
    #region Save
    private void SaveEdges(DialogueContainerSO _dialogueContainerSO)
    {
        // Delete pre-existing data
        _dialogueContainerSO.nodeLinkDatas.Clear();

        // Array of edges where edges have connections
        Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
        for (int i = 0; i < connectedEdges.Count(); i++)
        {
            BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
            BaseNode inputNode = (BaseNode)connectedEdges[i].input.node;

            _dialogueContainerSO.nodeLinkDatas.Add(new NodeLinkData 
            {
                // base node is the node where you come from, target node is the node connected
                baseNodeGuid = outputNode.NodeGuid,
                targetNodeGuid = inputNode.NodeGuid,
            });

        }
    }

    private void SaveNodes(DialogueContainerSO _dialogueContainerSO)
    {
        // Delete pre-existing data
        _dialogueContainerSO.dialogueNodeDatas.Clear();
        _dialogueContainerSO.eventNodeDatas.Clear();
        _dialogueContainerSO.startNodeDatas.Clear();
        _dialogueContainerSO.endNodeDatas.Clear();

        // Save new data
        foreach (BaseNode node in nodes)
        {
            switch (node)
            {
                case DialogueNode dialogueNode:
                    //Add node information to stored data 
                    _dialogueContainerSO.dialogueNodeDatas.Add(SaveNodeData(dialogueNode));
                    break;
                case EventNode eventNode:
                    //Add node information to stored data 
                    _dialogueContainerSO.eventNodeDatas.Add(SaveNodeData(eventNode));
                    break;
                case StartNode startNode:
                    //Add node information to stored data 
                    _dialogueContainerSO.startNodeDatas.Add(SaveNodeData(startNode));
                    break;
                case EndNode endNode:
                    //Add node information to stored data 
                    _dialogueContainerSO.endNodeDatas.Add(SaveNodeData(endNode));
                    break;
                default:
                    break;
            }
        }
    }
    private DialogueNodeData SaveNodeData(DialogueNode _Node)
    {
        DialogueNodeData dialoguenodedata = new DialogueNodeData
        {
            nodeguid = _Node.NodeGuid,
            position = _Node.GetPosition().position,
            key = _Node.Text,
            name = _Node.Name,
            //Audioclips = _node.AudioClips,
            dialoguefaceimagetype = _Node.FacingDirection,
            sprite = _Node.FaceImage,
            dialogueNodePorts = new List<DialogueNodePort>(_Node.DialogueNodePorts),
        };

        // going through each of the dialogue choice ports
        foreach (DialogueNodePort  nodeport in dialoguenodedata.dialogueNodePorts)
        {
            //empty data for error checking later
            nodeport.OutputGuid = string.Empty;
            nodeport.InputGuid = string.Empty;
            // check each edge
            foreach (Edge edge in edges)
            {
                // check if any is connected
                if(edge.output == nodeport.MyPort)
                {
                    // save info connection
                    nodeport.OutputGuid = (edge.output.node as BaseNode).NodeGuid;
                    nodeport.InputGuid = (edge.input.node as BaseNode).NodeGuid;
                }
            }
        }

        return dialoguenodedata;
    }

    private StartNodeData SaveNodeData(StartNode _node)
    {
        StartNodeData nodeData = new StartNodeData()
        {
            nodeguid = _node.NodeGuid,
            node_id = _node.IdStartNode,
            mainStartNode = _node.IsMain,
            position = _node.GetPosition().position,
        };
        return nodeData;
    }

    private EndNodeData SaveNodeData(EndNode _node)
    {
        EndNodeData nodeData = new EndNodeData()
        {
            nodeguid = _node.NodeGuid,
            position = _node.GetPosition().position,
            endNodeTypes = _node.EndNodeType,
        };
        return nodeData;
    }
    private EventNodeData SaveNodeData(EventNode _node)
    {
        EventNodeData nodeData = new EventNodeData()
        {
            nodeguid = _node.NodeGuid,
            position = _node.GetPosition().position,
            DialogueEventSo = _node.DialogueEvent,
        };
        return nodeData;
    }
    #endregion
    #region load
    public void Load(DialogueContainerSO _dialogueContainerSO)
    {
        ClearGraph();
        GenerateNodes(_dialogueContainerSO);
        ConnectNodes(_dialogueContainerSO);
    }

    private void ClearGraph()
    {
        edges.ForEach(edge => graphView.RemoveElement(edge));
        foreach (BaseNode node in nodes)
        {
            graphView.RemoveElement(node);
        }
    }

    private void GenerateNodes(DialogueContainerSO _dialogueContainerSO)
    {
        // Start
        foreach (StartNodeData node in _dialogueContainerSO.startNodeDatas)
        {
            StartNode tempNode = graphView.CreateStartNode(node.position);
            tempNode.NodeGuid = node.nodeguid;
            tempNode.IsMain = node.mainStartNode;
            tempNode.IdStartNode = node.node_id;
            //Update visuals with loaded values
            tempNode.LoadValueIntoField();
            graphView.AddElement(tempNode);
        }
        // End
        foreach (EndNodeData node in _dialogueContainerSO.endNodeDatas)
        {
            EndNode tempNode = graphView.CreateEndNode(node.position);
            tempNode.NodeGuid = node.nodeguid;
            tempNode.EndNodeType = node.endNodeTypes;
            //Update visuals with loaded values
            tempNode.LoadValueIntoField();
            graphView.AddElement(tempNode);
        }
        // Event
        foreach (EventNodeData node in _dialogueContainerSO.eventNodeDatas)
        {
            EventNode tempNode = graphView.CreateEventNode(node.position);
            tempNode.NodeGuid = node.nodeguid;
            tempNode.DialogueEvent = node.DialogueEventSo;
            //Update visuals with loaded values
            tempNode.LoadValueIntoField();
            graphView.AddElement(tempNode);
        }
        // Dialogue
        foreach (DialogueNodeData node in _dialogueContainerSO.dialogueNodeDatas)
        {
            DialogueNode tempNode = graphView.CreateDialogueNode(node.position);
            tempNode.NodeGuid = node.nodeguid;
            tempNode.Name = node.name;
            tempNode.Text = node.key;
            tempNode.FaceImage = node.sprite;
            tempNode.FacingDirection = node.dialoguefaceimagetype;
            //tempNode.audioClip = node.audioclips

            // Load choice ports
            foreach (DialogueNodePort nodeport in node.dialogueNodePorts)
            {
                tempNode.AddChoicePort(tempNode, nodeport);
            }
            //Update visuals with loaded values
            tempNode.LoadValueIntoField();
            graphView.AddElement(tempNode);
        }
    }

    private void ConnectNodes(DialogueContainerSO _dialogueContainerSO)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            //Collection of all connections of current node
            List<NodeLinkData> connections = _dialogueContainerSO.nodeLinkDatas.Where(edge => edge.baseNodeGuid == nodes[i].NodeGuid).ToList();
            for (int j = 0; j < connections.Count; j++)
            {
                string targetnodeguid = connections[j].targetNodeGuid;
                // finding the first instead of going through the entire list
                BaseNode targetnode = nodes.First(node => node.NodeGuid == targetnodeguid);
                // we do not want to do the connections of the dialogue just yet as it would put the choice connections in the wrong order
                if((nodes[i] is DialogueNode) == false)
                {
                    // Q is a function from visual elements, from what i understand it is a query to find the port but might need to search more
                    //The Q<T> method is shorthand for querying child elements within a UI container (VisualElement).
                    // It retrieves the first child element of type T that matches the criteria
                    LinkNodesTogether(nodes[i].outputContainer[j].Q<Port>(), (Port)targetnode.inputContainer[0]);
                }
            }
        }
        //                            find all nodes where        node is of type dialogue node       then cast to list
        List<DialogueNode> dialogueNodes = nodes.FindAll(node => node is DialogueNode).Cast<DialogueNode>().ToList();

        //connection of dialogue nodes
        foreach (DialogueNode dialoguenode in dialogueNodes)
        {
            foreach (DialogueNodePort nodeport in dialoguenode.DialogueNodePorts)
            {
                // if it doesnt have a connection, we dont want to connect
                if(nodeport.InputGuid != string.Empty)
                {
                    BaseNode targetNode = nodes.First(Node => Node.NodeGuid == nodeport.InputGuid);
                    LinkNodesTogether(nodeport.MyPort, (Port)targetNode.inputContainer[0]);
                }
            }
        }
    }

    private void LinkNodesTogether(Port _outputPort, Port _inputPort)
    {
        Edge tempEdge = new Edge()
        {
            output = _outputPort,
            input = _inputPort,
        };
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        // Finally create the connection
        graphView.Add(tempEdge);
    }
    #endregion
}
