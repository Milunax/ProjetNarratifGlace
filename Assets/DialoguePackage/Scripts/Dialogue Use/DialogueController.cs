using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;


#if TranslationSystemImplemented
using LocalizationPackage;
#endif

public class DialogueController : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI;
    [Header("Text")]
    [SerializeField] private Text textName;
    [SerializeField] private Text textBox;
    [Header("Image")]
    [SerializeField] private Image LeftImage;
    [SerializeField] private GameObject LeftImageGO; // hiding the image and the container of the image
    [SerializeField] private Image RightImage;
    [SerializeField] private GameObject RightImageGO;
    [Header("Buttons")] // TODO optimize later on, this is fugly
    [SerializeField] private Button button01;
    [SerializeField] private Text textbutton01;
    [Space]
    [SerializeField] private Button button02;
    [SerializeField] private Text textbutton02;
    [Space]
    [SerializeField] private Button button03;
    [SerializeField] private Text textbutton03;
    [Space]
    [SerializeField] private Button button04;
    [SerializeField] private Text textbutton04;


    private List<Button> buttons = new List<Button>();
    private List<Text> buttonTexts = new List<Text>();



    private string savedTextKey;
    private string savedNameKey;
    private string refTsv;
    private List<string> savedButtonKeys= new List<string>();

    public string RefTsv { get => refTsv; set => refTsv = value; }



#if TranslationSystemImplemented
    private void OnEnable()
    {
        LocalizationManager.OnRefresh += RefreshTexts;
    }


    private void OnDisable()
    {
        LocalizationManager.OnRefresh -= RefreshTexts;
    }

    private void RefreshTexts(SystemLanguage currentLanguage)
    {
        SetText(LocalizationManager.Instance.UniGetText(refTsv, savedNameKey), 
            LocalizationManager.Instance.UniGetText(refTsv, savedTextKey));
        RefreshButtons(savedButtonKeys);
    }
    public void RefreshButtons(List<string> _texts)
    {
        for (int i = 0; i < _texts.Count; i++)
        {
            buttonTexts[i].text = LocalizationManager.Instance.UniGetText(refTsv, _texts[i]);
        }
    }
#endif
    private void Awake()
    {
        ShowDialogue(false);
        // veryyyyyyyy ugly
        buttons.Add(button01);
        buttons.Add(button02);
        buttons.Add(button03);
        buttons.Add(button04);

        buttonTexts.Add(textbutton01);
        buttonTexts.Add(textbutton02);
        buttonTexts.Add(textbutton03);
        buttonTexts.Add(textbutton04);
    }

    public void ShowDialogue(bool _show)
    {
        dialogueUI.SetActive(_show);
    }

    // Ici serait l'endroit ou l'on envoie le dialogue récupéré avec la clé dans les database
    public void SetText(string _name, string _textbox)
    {
        textName.text = _name;
        textBox.text = _textbox;
    }

    public void SetImage(Sprite _image, DialogueSpriteType _dialogueSpriteType)
    {
        LeftImageGO.SetActive(false);
        RightImageGO.SetActive(false);

        if(_image != null)
        {
            if(_dialogueSpriteType == DialogueSpriteType.left)
            {
                LeftImage.sprite = _image;
                LeftImageGO.SetActive(true);
            } else
            {
                RightImage.sprite = _image;
                RightImageGO.SetActive(true);
            }
        }
    }

    public void SetButtons(List<string> _texts, List<UnityAction> _unityActions)
    {
        buttons.ForEach(button => button.gameObject.SetActive(false));

        for (int i = 0; i < _texts.Count; i++)
        {
            Debug.Log("text button number " + i + ": " + _texts[i]);
            buttonTexts[i].text = _texts[i];
            buttons[i].gameObject.SetActive(true);
            // resets the listeners
            buttons[i].onClick = new Button.ButtonClickedEvent();
            // add a delegate
            buttons[i].onClick.AddListener(_unityActions[i]);
        }
    }
}