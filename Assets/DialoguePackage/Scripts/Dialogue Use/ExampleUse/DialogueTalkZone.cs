using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTalkZone : MonoBehaviour
{
    //[SerializeField] private GameObject speechBubble;
    [SerializeField] private KeyCode talkKey = KeyCode.E;
    [SerializeField] private KeyCode talkKeyDelayed = KeyCode.F;
    [SerializeField] private string idStartDelayed = "Delayed";
    //[SerializeField] private Text keyInputText;

    [SerializeField] private DialogueTalk DialogueTalk;

    private void Awake()
    {
        //speechBubble.SetActive(true);
        //keyInputText.text = talkKey.ToString();

        //DialogueTalk = GetComponent<DialogueTalk>();
    }

    private void OnEnable()
    {
        DialogueTalk.StartDialogue();
    }


    void Update()
    {
        if (Input.GetKeyDown(talkKey) /*&& speechBubble.activeSelf */ && DialogueTalk != null)
        {
            
        }
        if (Input.GetKeyDown(talkKeyDelayed) /*&& speechBubble.activeSelf */ && DialogueTalk != null)
        {
            DialogueTalk.StartDialogue(idStartDelayed);
        }
    }
}