using NaughtyAttributes;
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
    }
    private void OnDisable()
    {
        DirectionalPad.OnKeyPressed -= DirectionnalPadInput;
    }

    private void Start()
    {
        _renderCamera.targetTexture.height = sizeInPixel;
        _renderCamera.targetTexture.width = sizeInPixel;

        _rectScreens.localScale = new Vector2(sizeInPixel / 256, sizeInPixel / 256);

        OpenDialogue(true);
    }

    public void OpenMap()
    {
        if (_activeScreen != SCREEN_ACTIVE.SHUTDOWN &&  _activeScreen <= SCREEN_ACTIVE.BLACK_SCREEN)
        {
            _activeScreen = SCREEN_ACTIVE.MAP;
        }
        _activeMap = MAP_ACTIVE.MAP;

        //Open Map
    }
    public void CloseMap()
    {
        if (_activeScreen != SCREEN_ACTIVE.SHUTDOWN && _activeScreen <= SCREEN_ACTIVE.BLACK_SCREEN)
        {
            _activeScreen = SCREEN_ACTIVE.BLACK_SCREEN;
        }
        _activeMap = MAP_ACTIVE.CLOSED;

        //_Map.Closing();
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

        //_fileExplorer.Closing();
    }

    public void OpenDialogue(bool isInteractif)
    {
        if (isInteractif && _activeScreen != SCREEN_ACTIVE.SHUTDOWN)
        {
            _activeScreen = SCREEN_ACTIVE.DIALOGUE;
        }

        if (isInteractif) _activeDialogue = DIALOGUE_ACTIVE.DIALOGUE_INTERACTIVE;
        else _activeDialogue = DIALOGUE_ACTIVE.DIALOGUE_NO_INTERACTIVE;

        _dialogueController.Opening();
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

        //Close Dialogues
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


}
