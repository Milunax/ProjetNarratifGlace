using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGetData : MonoBehaviour
{

    [SerializeField] protected DialogueContainerSO dialogueContainer;

    protected BaseNodeData GetNodeByGuid(string _targetNodeGuid)
    {
        //find node that has the same uid
        return dialogueContainer.AllNodes.Find(node => node.nodeguid == _targetNodeGuid);
    }

    protected BaseNodeData GetNodeByNodePort(DialogueNodePort nodePort)
    {
        return dialogueContainer.AllNodes.Find(node => node.nodeguid == nodePort.InputGuid);
    }


    protected BaseNodeData GetNextNode(BaseNodeData _baseNodeData)
    {
        NodeLinkData nodeLinkData = dialogueContainer.nodeLinkDatas.Find(edge => edge.baseNodeGuid == _baseNodeData.nodeguid);

        return GetNodeByGuid(nodeLinkData.targetNodeGuid);
    }

}
