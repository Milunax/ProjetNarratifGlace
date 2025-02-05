using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class ScreenDisplay : MonoBehaviour
{
    [SerializeField] private Camera _renderCamera;
    [SerializeField] private MeshRenderer _renderMesh;
    [SerializeField] private RectTransform _rectScreens;

    [Space(10)]
    [Header("Screens")]
    [SerializeField] private FileExplorerBase _fileExplorer;
    [SerializeField] private DialogueController _dialogueController;
    [SerializeField] private Map _map;


    [Header("Resolution")]
    [InfoBox("Size can't be below 256 pixels")]
    [SerializeField] private int sizeInPixel = 1024;

    [Header("Status")]
    [SerializeField] private SCREEN_ACTIVE _activeScreen = SCREEN_ACTIVE.MAP;
    [SerializeField, ReadOnly] private MAP_ACTIVE _activeMap = MAP_ACTIVE.MAP;
    [SerializeField, ReadOnly] private FILE_EXPLORER_ACTIVE _activeFileExplorer = FILE_EXPLORER_ACTIVE.CLOSED;
    [SerializeField, ReadOnly] private DIALOGUE_ACTIVE _activeDialogue = DIALOGUE_ACTIVE.CLOSED;

    [Header("Dialogue graph events tags")]
    [SerializeField] private string _openDialogueTag;
    [SerializeField] private string _closeDialogueTag;
    [SerializeField] private string _openMap;
    [SerializeField] private string _closeMap;
    [SerializeField] private string _openFileExplorer;
    [SerializeField] private string _closeFileExplorer;

    private void OnValidate()
    {
        if (sizeInPixel < 256)
        {
            sizeInPixel = 256;
        }
    }

    private void OnEnable()
    {
        DirectionalPad.OnKeyPressed += DirectionnalPadInput;
        ContextualButtons.OnKeyPressed += ContextualPadInput;
    }
    private void OnDisable()
    {
        DirectionalPad.OnKeyPressed -= DirectionnalPadInput;
        ContextualButtons.OnKeyPressed -= ContextualPadInput;
    }

    private void Start()
    {
        DialogueEventSO.DialogueEvent += GraphEvent;
        _renderCamera.targetTexture.height = sizeInPixel;
        _renderCamera.targetTexture.width = sizeInPixel;

        _rectScreens.localScale = new Vector2(sizeInPixel / 256, sizeInPixel / 256);

        CloseDialogue();
        CloseMap();
        CloseFileExplorer();
    }

    private void GraphEvent(string tag, bool isDialogueInteractive)
    {
        switch (tag)
        {
            case "OpenDialogue":
                Debug.Log("Opendialogue");
                OpenDialogue(isDialogueInteractive);
                break;
            case "CloseDialogue":
                CloseDialogue();
                break;
            case "OpenMap":
                Debug.Log("OpenMap");
                OpenMap();
                break;
            case "CloseMap":
                CloseMap();
                break;
            case "OpenFileExplorer":
                OpenFileExplorer();
                break;
            case "CloseFileExplorer":
                CloseFileExplorer();
                break;
        }
    }

    public void OpenMap()
    {
        if (_activeScreen != SCREEN_ACTIVE.SHUTDOWN &&  _activeScreen <= SCREEN_ACTIVE.BLACK_SCREEN)
        {
            _activeScreen = SCREEN_ACTIVE.MAP;
        }
        _activeMap = MAP_ACTIVE.MAP;

        _map.Opening();
    }
    public void CloseMap()
    {
        if (_activeScreen != SCREEN_ACTIVE.SHUTDOWN && _activeScreen <= SCREEN_ACTIVE.BLACK_SCREEN)
        {
            _activeScreen = SCREEN_ACTIVE.BLACK_SCREEN;
        }
        _activeMap = MAP_ACTIVE.CLOSED;

        _map.Closing();
    }

    private void FileExplorerButton()
    {
        if (_activeFileExplorer == FILE_EXPLORER_ACTIVE.CLOSED)
        {
            OpenFileExplorer();
        }
        else if (_activeFileExplorer != FILE_EXPLORER_ACTIVE.CLOSED)
        {
            CloseFileExplorer();
        }
    }
    public void OpenFileExplorer()
    {
        if (_activeScreen != SCREEN_ACTIVE.SHUTDOWN && _activeScreen <= SCREEN_ACTIVE.MAP)
        {
            _activeScreen = SCREEN_ACTIVE.FILE_EXPLORER;
        }
        _activeFileExplorer = FILE_EXPLORER_ACTIVE.FILE_EXPLORER;

        _fileExplorer.Opening();
    }
    public void CloseFileExplorer()
    {
        if (_activeScreen != SCREEN_ACTIVE.SHUTDOWN && (_activeDialogue <= DIALOGUE_ACTIVE.DIALOGUE_NO_INTERACTIVE || _activeScreen < SCREEN_ACTIVE.DIALOGUE))
        {
            _activeScreen = SCREEN_ACTIVE.MAP;
        }
        _activeFileExplorer = FILE_EXPLORER_ACTIVE.CLOSED;

        _fileExplorer.Closing();
    }

    public void OpenDialogue(bool isInteractif)
    {
        //Debug.Log("test");
        if (isInteractif && _activeScreen != SCREEN_ACTIVE.SHUTDOWN)
        {
            _activeScreen = SCREEN_ACTIVE.DIALOGUE;
        }

        if (isInteractif) _activeDialogue = DIALOGUE_ACTIVE.DIALOGUE_INTERACTIVE;
        else _activeDialogue = DIALOGUE_ACTIVE.DIALOGUE_NO_INTERACTIVE;

        //_dialogueController.Opening();
    }
    public void CloseDialogue()
    {
        if (_activeScreen != SCREEN_ACTIVE.SHUTDOWN)
        {
            if (_activeFileExplorer != FILE_EXPLORER_ACTIVE.CLOSED) _activeScreen = SCREEN_ACTIVE.FILE_EXPLORER;
            else if (_activeMap != MAP_ACTIVE.CLOSED) _activeScreen = SCREEN_ACTIVE.MAP;
            else _activeScreen = SCREEN_ACTIVE.BLACK_SCREEN;
        }

        _activeDialogue = DIALOGUE_ACTIVE.CLOSED;

        //_dialogueController.Closing();
    }
    public void ChangeDialogueState(DIALOGUE_ACTIVE state)
    {
        if (state == DIALOGUE_ACTIVE.CLOSED) CloseDialogue();
        _activeDialogue = state;

        if (_activeScreen == SCREEN_ACTIVE.SHUTDOWN) return;

        if (_activeDialogue == DIALOGUE_ACTIVE.DIALOGUE_INTERACTIVE)
        {
            _activeScreen = SCREEN_ACTIVE.DIALOGUE;
        }
        else if (_activeDialogue == DIALOGUE_ACTIVE.DIALOGUE_NO_INTERACTIVE)
        {
            if (_activeFileExplorer != FILE_EXPLORER_ACTIVE.CLOSED) _activeScreen = SCREEN_ACTIVE.FILE_EXPLORER;
            else if (_activeMap != MAP_ACTIVE.CLOSED) _activeScreen = SCREEN_ACTIVE.MAP;
            else _activeScreen = SCREEN_ACTIVE.BLACK_SCREEN;
        }
    }

    private void DirectionnalPadInput(DIRECTIONAL_PAD_INFO input)
    {
        switch (_activeScreen)
        {
            case SCREEN_ACTIVE.MAP:
                {
                    _map.ReceiveInput(input);
                }
                break;
            case SCREEN_ACTIVE.FILE_EXPLORER:
                {
                    _fileExplorer.InputsListener(input);
                }
                break;
            case SCREEN_ACTIVE.DIALOGUE:
                {
                    _dialogueController.ReceiveDirectionalInput(input);
                }
                break;

            default:
                break;
        }
    }
    private void ContextualPadInput(CONTEXTUAL_INPUT_INFO input)
    {
        if (_activeScreen > SCREEN_ACTIVE.BLACK_SCREEN && input == CONTEXTUAL_INPUT_INFO.FILE_EXPLORER)
        {
            FileExplorerButton();
        }

        switch (_activeScreen)
        {
            case SCREEN_ACTIVE.MAP:
                {

                }
                break;
            case SCREEN_ACTIVE.FILE_EXPLORER:
                {
                    
                }
                break;
            case SCREEN_ACTIVE.DIALOGUE:
                {

                }
                break;
            default:
                break;
        }
    }

}
