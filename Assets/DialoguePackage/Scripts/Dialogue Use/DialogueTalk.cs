using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

#if TranslationSystemImplemented
using LocalizationPackage;
#endif

public class DialogueTalk : DialogueGetData
{
    [SerializeField] private DialogueController dialogueController;
    #if TranslationSystemImplemented
    [SerializeField] private string RefTsvLocalisation;
    #endif
    //[SerializeField] private AudioSource audioSource;
    
    private DialogueNodeData currentDialogueNodeData;
    private DialogueNodeData lastDialogueNodeData;

    public void StartDialogue()
    {
        // look for next node to execute
        CheckNodeType(GetNextNode(dialogueContainer.startNodeDatas[0]));
        dialogueController.ShowDialogue(true);
    }

    public void StartDialogue(SystemLanguage language)
    {
        // look for next node to execute
        CheckNodeType(GetNextNode(dialogueContainer.startNodeDatas[0]));
        dialogueController.ShowDialogue(true);
    }

    public void StartDialogue(string IDStart)
    {
        // look for a node that matches the id sent
        foreach (var item in dialogueContainer.startNodeDatas)
        {
            Debug.Log("Node Start ids: " + item.node_id);
            if(item.node_id == IDStart)
            {
                CheckNodeType(GetNextNode(item));
                dialogueController.ShowDialogue(true);
                return;
            }
        }
        // if none found, take the first startNodeData
        CheckNodeType(GetNextNode(dialogueContainer.startNodeDatas[0]));
        dialogueController.ShowDialogue(true);
    }

    private void CheckNodeType(BaseNodeData baseNodeData)
    {
        switch (baseNodeData)
        {
            case StartNodeData nodeData:
                RunNode(nodeData);
                break;
            case DialogueNodeData nodeData:
                RunNode(nodeData);
                break;
            case EventNodeData nodeData:
                RunNode(nodeData);
                break;
            case EndNodeData nodeData:
                RunNode(nodeData);
                break;
            default:
                break;
        }
    }

    private void RunNode(StartNodeData nodeData)
    {
        // look for next node to execute
        CheckNodeType(GetNextNode(dialogueContainer.startNodeDatas[0]));
    }

#if !TranslationSystemImplemented
    private void Awake()
    {
        dialogueController = FindObjectOfType<DialogueController>();
        // audioSource=GetComponent<AudioSourfce>();
    }

    private void RunNode(DialogueNodeData nodeData)
    {
        lastDialogueNodeData = currentDialogueNodeData;
        currentDialogueNodeData = nodeData;
        // This here is where you should try and find the text from the database and send it as second value !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        dialogueController.SetText(nodeData.name, nodeData.key);
        dialogueController.SetImage(nodeData.sprite, nodeData.dialoguefaceimagetype);
        MakeButtons(nodeData.dialogueNodePorts);

        //audioSource.clip = (...)
        //audioSource.Play();
    }

        private void MakeButtons(List<DialogueNodePort> _nodePorts)
    {
        List<string> texts = new List<string>();
        List<UnityAction> unityActions = new List<UnityAction>();

        foreach (DialogueNodePort nodeport in _nodePorts)
        {
            texts.Add(nodeport.Key);
            UnityAction tempAction = null;
            // Add a method into action
            tempAction += () =>
            {
                // when action is called, execute stuff between these brackets
                CheckNodeType(GetNodeByGuid(nodeport.InputGuid));
                //audioSource.Stop();
            };
            unityActions.Add(tempAction);
        }
        dialogueController.SetButtons(texts, unityActions);
    }
#endif

#if TranslationSystemImplemented
    private void Awake()
    {
        dialogueController = FindObjectOfType<DialogueController>();
        dialogueController.RefTsv = RefTsvLocalisation;
        // audioSource=GetComponent<AudioSourfce>();
    }

    private void RunNode(DialogueNodeData nodeData)
    {
        lastDialogueNodeData = currentDialogueNodeData;
        currentDialogueNodeData = nodeData;
        // donc en utilisant le tsv et le Localization package, je lui envoie la clé et recupère une valeur
        dialogueController.SetText(
            LocalizationManager.Instance.UniGetText(RefTsvLocalisation, nodeData.name),
            LocalizationManager.Instance.UniGetText(RefTsvLocalisation, nodeData.key)
            );
        dialogueController.SetImage(nodeData.sprite, nodeData.dialoguefaceimagetype);
        MakeButtons(nodeData.dialogueNodePorts);
        //audioSource.clip = (...)
        //audioSource.Play();
    }

    private void MakeButtons(List<DialogueNodePort> _nodePorts)
    {
        List<string> texts = new List<string>();
        List<UnityAction> unityActions = new List<UnityAction>();

        foreach (DialogueNodePort nodeport in _nodePorts)
        {
            texts.Add(LocalizationManager.Instance.UniGetText(RefTsvLocalisation, nodeport.Key));
            UnityAction tempAction = null;
            // Add a method into action
            tempAction += () =>
            {
                // when action is called, execute stuff between these brackets
                CheckNodeType(GetNodeByGuid(nodeport.InputGuid));
                Debug.Log(nodeport.InputGuid);
                Debug.Log("Delegate Called");
                //audioSource.Stop();
            };
            unityActions.Add(tempAction);
        }
        dialogueController.SetButtons(texts, unityActions);
    }
#endif 
    private void RunNode(EventNodeData nodeData)
    {
        if (nodeData.DialogueEventSo != null)
        {
            nodeData.DialogueEventSo.RunEvent(nodeData.tag);
        }
        CheckNodeType(GetNextNode(nodeData));
    }
    private void RunNode(EndNodeData nodeData)
    {
        switch (nodeData.endNodeTypes)
        {
            case EndNodeTypes.End:
                dialogueController.ShowDialogue(false);
                break;
            case EndNodeTypes.Repeat:
                CheckNodeType(GetNodeByGuid(currentDialogueNodeData.nodeguid));
                break;
            case EndNodeTypes.GoBack:
                CheckNodeType(GetNodeByGuid(lastDialogueNodeData.nodeguid));
                break;
            case EndNodeTypes.ReturnStart:
                CheckNodeType(GetNextNode(dialogueContainer.startNodeDatas[0]));
                break;
            default:
                break;
        }
    }
}
