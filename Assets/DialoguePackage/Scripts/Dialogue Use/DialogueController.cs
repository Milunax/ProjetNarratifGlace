using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Runtime.CompilerServices;



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

    [Space]

    [SerializeField] private Button _selectedButton = null;
    [SerializeField] private List<Button> _activeButtonsList = new List<Button>();
    public Action<Button, Color> OnSelectedButtonValueChange;

    [SerializeField] private Image _selectionImage;

    private List<Button> buttons = new List<Button>();
    private List<Text> buttonTexts = new List<Text>();



    private string savedTextKey;
    private string savedNameKey;
    private string refTsv;
    private List<string> savedButtonKeys= new List<string>();

    public string RefTsv { get => refTsv; set => refTsv = value; }
    public Button SelectedButton { get => _selectedButton; set { _selectedButton = value; OnSelectedButtonValueChange?.Invoke(_selectedButton, Color.green); } }



#if TranslationSystemImplemented
    private void OnEnable()
    {
        LocalizationManager.OnRefresh += RefreshTexts;
        //DirectionalPad.OnKeyPressed += ReceiveDirectionalInput;
        OnSelectedButtonValueChange += UpdateButtonVisual;
        OnSelectedButtonValueChange += MoveSelectionImage;
    }

    private void OnDisable()
    {
        LocalizationManager.OnRefresh -= RefreshTexts;
        //DirectionalPad.OnKeyPressed -= ReceiveDirectionalInput;
        OnSelectedButtonValueChange -= UpdateButtonVisual;
        OnSelectedButtonValueChange -= MoveSelectionImage;
    }

    public void Opening()
    {
        ShowDialogue(true);
    }

    public void Closing()
    {
        ShowDialogue(false);
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
        //ShowDialogue(false);
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
        Debug.Log(_show);
        dialogueUI.SetActive(_show);
    }

    // Ici serait l'endroit ou l'on envoie le dialogue récupéré avec la clé dans les database
    public void SetText(string _name, string _textbox)
    {
        if(textName != null) textName.text = _name;
        if(textBox != null) textBox.text = _textbox;
    }

    public void SetImage(Sprite _image, DialogueSpriteType _dialogueSpriteType)
    {
        if(LeftImageGO != null) LeftImageGO.SetActive(false);
        if(RightImageGO != null) RightImageGO.SetActive(false);

        if(_image != null)
        {
            if(_dialogueSpriteType == DialogueSpriteType.left)
            {
                if (LeftImage != null) LeftImage.sprite = _image;
                if (LeftImageGO != null) LeftImageGO.SetActive(true);
            } else
            {
                if (RightImage != null) RightImage.sprite = _image;
                if (RightImageGO != null) RightImageGO.SetActive(true);
            }
        }
    }

    public void SetButtons(List<string> _texts, List<UnityAction> _unityActions)
    {
        _activeButtonsList.Clear();
        buttons.ForEach(button => button.gameObject.SetActive(false));

        Debug.Log(_unityActions[0].GetInvocationList());

        for (int i = 0; i < _texts.Count; i++)
        {
            _activeButtonsList.Add(buttons[i]);
            Debug.Log("text button number " + i + ": " + _texts[i]);
            buttonTexts[i].text = _texts[i];
            buttons[i].gameObject.SetActive(true);
            // resets the listeners
            buttons[i].onClick = new Button.ButtonClickedEvent();
            // add a delegate
            buttons[i].onClick.AddListener(_unityActions[i]);
            
        }
        SelectedButton = _activeButtonsList[0];
        Debug.Log(SelectedButton.transform.position.y);
    }

    public void ReceiveDirectionalInput(DIRECTIONAL_PAD_INFO input)
    {
        int idCurrentButton;
        switch (input)
        {
            //ADD SECURITIES TO NOT GO OUT OF BOUNDS OF LIST
            case DIRECTIONAL_PAD_INFO.UP:
                idCurrentButton = _activeButtonsList.IndexOf(SelectedButton);
                Debug.Log("UP : " + SelectedButton.name);
                Debug.Log("UP : " + idCurrentButton);
                if (idCurrentButton > 0)
                {
                    UpdateButtonVisual(_selectedButton, Color.white);
                    SelectedButton = _activeButtonsList[idCurrentButton - 1];
                }
                break;

            case DIRECTIONAL_PAD_INFO.DOWN:
                idCurrentButton = _activeButtonsList.IndexOf(SelectedButton);
                Debug.Log("DOWN : " + SelectedButton.name);
                Debug.Log("DOWN : " + idCurrentButton);
                if (idCurrentButton < _activeButtonsList.Count - 1)
                {
                    UpdateButtonVisual(_selectedButton, Color.white);
                    SelectedButton = _activeButtonsList[idCurrentButton + 1];
                }
                break;

            case DIRECTIONAL_PAD_INFO.CONFIRM:
                UpdateButtonVisual(_selectedButton, Color.white);
                SelectedButton.onClick?.Invoke();
                break;

            default:
                break;
        }
    }

    /*private void UpdateButtonVisual()
    {
        var buttonColors = SelectedButton.colors;
        buttonColors.normalColor = Color.green;
        SelectedButton.colors = buttonColors;
    }*/


    private void UpdateButtonVisual(Button button, Color color)
    {
        var buttonColors = button.colors;
        buttonColors.normalColor = color;
        button.colors = buttonColors;
    }

    private void MoveSelectionImage(Button button, Color color)
    {
        //Debug.Log(button.transform.position.y);
        _selectionImage.rectTransform.position = new Vector3(_selectionImage.rectTransform.position.x, button.transform.position.y, _selectionImage.rectTransform.position.z);
    }
}