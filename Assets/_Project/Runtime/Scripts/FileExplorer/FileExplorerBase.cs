using System.Collections.Generic;
using UnityEngine;
using GMSpace;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using LocalizationPackage;

public class FileExplorerBase : MonoBehaviour
{
    [Header("Utilities")]
    [SerializeField] private GameObject _textContainer;
    private RectTransform _rectTextContainer;
    [SerializeField] private GameObject _textLinePrefab;
    private float GetTextContainerSize { get => _rectTextContainer.sizeDelta.x; }
    [SerializeField] private LocalizationComponent _localComponent;
    [SerializeField, ReadOnly] private FILE_EXPLORER_ACTIVE _currentScreen;

    [Header("Childs UI")]
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _textFileUI;
    [SerializeField] private FileExplorerTextUI _textFileUIScript;
    [SerializeField] private WheelBehaviour _audioFileUIScript;

    [Header("Display Paddings")]
    [SerializeField] private Vector2 _topLeftPadding;
    [SerializeField] private float _linePadding = 10f;
    [SerializeField] private float _textSize = 20f;

    [Header("Display Indicators")]
    [SerializeField] private TextMeshProUGUI _folderIndicator;
    [SerializeField] private Image _topIndicator;
    [SerializeField] private Image _bottomIndicator;
    private int _skippedSteps = 0;
    private float _topPosY;
    private float _bottomPosY;
    private int _maxLineOnScreen;

    public float GetSetTextSize
    {
        get => _textSize;
        set => _textSize = value;
    }


    [Space(15)]
    [Header("Root Data")]
    [SerializeField] private List<FileExplorerDataSO> _rootData = new List<FileExplorerDataSO>();

    private List<string> _dataPath = new List<string>();
    private List<DISPLAY_DATA> _displayedData = new List<DISPLAY_DATA>();
    private int _cursor;

    private struct DISPLAY_DATA
    {
        public FileExplorerDataSO data;
        public FILE_TYPE type;
        public bool isBlocked;
        public FileExplorerLine textLine;

        public DISPLAY_DATA(FileExplorerDataSO newData, FILE_TYPE newType, bool blocked)
        {
            data = newData;
            type = newType;
            isBlocked = blocked;
            textLine = null;
        }
    }


    private void Start()
    {
        TryGetComponent<RectTransform>(out _rectTextContainer);
        SetupTextDisplayValues();
        _textFileUIScript.SetTextValues(_textSize, _topLeftPadding);
        _audioFileUIScript.SetTextValues(_textSize);

        _background.SetActive(false);
        _textContainer.SetActive(false);
        _textFileUI.SetActive(false);
        _audioFileUIScript.gameObject.SetActive(false);
    }

    public void Closing()
    {
        _dataPath.Clear();
        _cursor = 0;

        foreach (DISPLAY_DATA item in _displayedData)
        {
            item.textLine.Delete();
        }
        _displayedData.Clear();

        _background.SetActive(false);
        _textContainer.SetActive(false);
        _textFileUI.SetActive(false);
        _audioFileUIScript.gameObject.SetActive(false);

        _currentScreen = FILE_EXPLORER_ACTIVE.CLOSED;
    }

    public void Opening()
    {
        _background.SetActive(true);
        _textContainer.SetActive(true);
        _textFileUI.SetActive(false);
        _audioFileUIScript.gameObject.SetActive(false);

        _currentScreen = FILE_EXPLORER_ACTIVE.FILE_EXPLORER;

        SetDisplayData(_rootData);
    }

    private void SetDisplayData(List<FileExplorerDataSO> listOfData)
    {
        _cursor = 0;
        foreach (DISPLAY_DATA item in _displayedData)
        {
            item.textLine.Delete();
        }
        _displayedData.Clear();

        foreach (FileExplorerDataSO item in listOfData)
        {
            if (item == null ||
                item.isVisible.day < GameManager.Instance.GetSetProgressionDay &&
                item.isVisible.inDay < GameManager.Instance.GetSetProgressionInDay &&
                item.isVisible.narrativeIALevel < GameManager.Instance.GetSetNarrativeIALevel) continue; // if file/folder isn't visible

            FILE_TYPE type = GetFileType(item);

            if (item.isAccessible.day >= GameManager.Instance.GetSetProgressionDay &&
                item.isAccessible.inDay >= GameManager.Instance.GetSetProgressionInDay &&
                item.isAccessible.narrativeIALevel >= GameManager.Instance.GetSetNarrativeIALevel) // if file/folder is accessible
            {
                if (type == FILE_TYPE.FOLDER) // if folder
                {
                    FileExplorerFolderSO folder = (FileExplorerFolderSO)item;
                    for (int i = 0; i < folder.childs.Count; i++)
                    {
                        FileExplorerDataSO file = folder.childs[i];
                        if (file == null ||
                            file.isVisible.day < GameManager.Instance.GetSetProgressionDay &&
                            file.isVisible.inDay < GameManager.Instance.GetSetProgressionInDay &&
                            file.isVisible.narrativeIALevel < GameManager.Instance.GetSetNarrativeIALevel) folder.childs.RemoveAt(i); // if file in folder is empty, remove it
                    }

                    if (folder.childs.Count <= 0) { continue; } // if folder is empty, don't display
                }

                DISPLAY_DATA itemData = new DISPLAY_DATA(item, type, false);

                CreateTextLine(ref itemData);
                _displayedData.Add(itemData);
            }
            else // if file/folder isn't accessible
            {
                DISPLAY_DATA itemData = new DISPLAY_DATA(item, type, true);

                CreateTextLine(ref itemData);
                _displayedData.Add(itemData);
            }
        }

        for (int i = 0; i < _displayedData.Count; i++)
        {
            SetDisplayLines(_displayedData[i].textLine, i);
        }
        _displayedData[0].textLine.GetSetCursor = true;

        string indicator = "PC";
        if (_dataPath.Count < 2)
        {
            foreach (string folder in _dataPath)
            {
                indicator += "/" + folder;
            }
        }
        else
        {
            indicator += "/.." + (_dataPath.Count - 1) + "../" + _dataPath[_dataPath.Count - 1];
        }

        _folderIndicator.text = indicator;
    }
    private FILE_TYPE GetFileType(FileExplorerDataSO file)
    {
        if (file is FileExplorerFolderSO) return FILE_TYPE.FOLDER;
        if (file is FileExplorerFileTextSO) return FILE_TYPE.FILE_TEXT;
        if (file is FileExplorerAudioFileSO) return FILE_TYPE.FILE_AUDIO;
        return FILE_TYPE.NULL;
    }

    private void SetupTextDisplayValues()
    {
        _topPosY = -(_topLeftPadding.y + (_textSize * 1.5f));
        _bottomPosY = -512 - _topPosY;

        int count = 0;
        float temp = _topPosY;
        while (temp >= _bottomPosY)
        {
            count++;
            temp -= (_linePadding + _textSize);
        }

        _maxLineOnScreen = count;
        //Debug.Log(_topPosY + " | " + _bottomPosY + " | " + _maxLineOnScreen + " | " + (_linePadding + _textSize));

        _folderIndicator.fontSize = _textSize;
        _folderIndicator.rectTransform.sizeDelta = new Vector2(0, _textSize);
        _folderIndicator.rectTransform.localPosition = new Vector2((_textSize / 2f) + (_rectTextContainer.sizeDelta.x / 2f), 0);
        _folderIndicator.rectTransform.anchoredPosition = new Vector2(_topLeftPadding.x, - _topLeftPadding.y - (_textSize * 0.5f));
    }
    private bool CreateTextLine(ref DISPLAY_DATA itemData)
    {
        GameObject line = Instantiate(_textLinePrefab, _textContainer.transform);
        line.name = itemData.data.fileName;

        if (line.TryGetComponent<FileExplorerLine>(out itemData.textLine))
        {
            itemData.textLine.SetupVisual(_rectTextContainer.sizeDelta.x, _textSize, _localComponent.GetTextSafe(itemData.data.fileName), itemData.type, itemData.isBlocked);
            return true;
        }
        else
        {
            return false;
        }
    }
    private void SetDisplayLines(FileExplorerLine item, int line)
    {
        item.GetRectTransform.anchoredPosition = new Vector2(_topLeftPadding.x + (_textSize * 1.5f) + item.GetWidthSpacing,
                                                          - (_topLeftPadding.y * 2) - (_textSize * 1.5f) - ((_linePadding + _textSize) * line));

        SetVisibility(item, line);
    }
    private void SetVisibility(FileExplorerLine item, int line)
    {
        if (line >= _skippedSteps && line <= _maxLineOnScreen) item.SetVisibility(true);
        else item.SetVisibility(false);
    }

    public void InputsListener(DIRECTIONAL_PAD_INFO padInfo)
    {
        switch (_currentScreen)
        {
            case FILE_EXPLORER_ACTIVE.FILE_EXPLORER:
                {
                    switch (padInfo)
                    {
                        case DIRECTIONAL_PAD_INFO.UP:
                        case DIRECTIONAL_PAD_INFO.DOWN:
                            {
                                MoveCursor(padInfo);
                                break;
                            }

                        case DIRECTIONAL_PAD_INFO.CONFIRM:
                        case DIRECTIONAL_PAD_INFO.RIGHT:
                            {
                                OpenFile();
                                break;
                            }

                        case DIRECTIONAL_PAD_INFO.LEFT:
                            {
                                CloseFolder();
                                break;
                            }

                        default:
                            break;

                    }
                    break;
                }

            case FILE_EXPLORER_ACTIVE.TEXT:
                {
                    switch (padInfo)
                    {
                        case DIRECTIONAL_PAD_INFO.UP:
                        case DIRECTIONAL_PAD_INFO.DOWN:
                            {
                                _textFileUIScript.ScrollText(padInfo);
                                break;
                            }
                        case DIRECTIONAL_PAD_INFO.LEFT:
                            {
                                _textContainer.SetActive(true);
                                _textFileUI.SetActive(false);

                                _currentScreen = FILE_EXPLORER_ACTIVE.FILE_EXPLORER;
                                break;
                            }

                        default:
                            break;
                    }
                    break;
                }
            case FILE_EXPLORER_ACTIVE.AUDIO:
                {
                    switch (padInfo)
                    {
                        case DIRECTIONAL_PAD_INFO.LEFT:
                            {
                                _audioFileUIScript.Closing();

                                _textContainer.SetActive(true);
                                _audioFileUIScript.gameObject.SetActive(false);

                                _currentScreen = FILE_EXPLORER_ACTIVE.FILE_EXPLORER;
                                break;
                            }

                        default:
                            break;
                    }

                    break;
                }
        }

    }

    private void MoveCursor(DIRECTIONAL_PAD_INFO direction)
    {
        int add = 0;

        switch (direction)
        {
            case DIRECTIONAL_PAD_INFO.UP:
                add = -1;
                break;
            case DIRECTIONAL_PAD_INFO.DOWN:
                add = +1;
                break;

            default:
                return;
        }

        if (!GetCursorValidation(add)) return;

        _displayedData[_cursor].textLine.GetSetCursor = false;
        _cursor += add;
        _displayedData[_cursor].textLine.GetSetCursor = true;
    }
    private bool GetCursorValidation(int add)
    {
        int temp = _cursor + add;

        if (temp < 0 || temp >= _displayedData.Count) return false;
        else return true;
    }

    private void OpenFile()
    {
        if (_displayedData[_cursor].isBlocked)
        {
            BlockedFile();
            return;
        }

        switch (_displayedData[_cursor].type)
        {
            case FILE_TYPE.FOLDER:
                {
                    OpenFolder();
                    break;
                }
            case FILE_TYPE.FILE_TEXT:
                {
                    _textContainer.SetActive(false);
                    _textFileUI.SetActive(true);

                    FileExplorerFileTextSO textFile = (FileExplorerFileTextSO)_displayedData[_cursor].data;
                    _textFileUIScript.SetInfos(_localComponent.GetTextSafe(textFile.title), _localComponent.GetTextSafe(textFile.description), textFile.image);

                    _currentScreen = FILE_EXPLORER_ACTIVE.TEXT;

                    break;
                }
            case FILE_TYPE.FILE_AUDIO:
                {
                    _textContainer.SetActive(false);
                    _audioFileUIScript.gameObject.SetActive(true);

                    FileExplorerAudioFileSO audioFile = (FileExplorerAudioFileSO)_displayedData[_cursor].data;
                    _audioFileUIScript.Opening();
                    _audioFileUIScript.SetValues(audioFile.title, audioFile.transcriptions, audioFile.audio);

                    _currentScreen = FILE_EXPLORER_ACTIVE.AUDIO;

                    break;
                }

            default:
                BlockedFile();
                break;
        }
    }
    private void BlockedFile()
    {
        Debug.Log("File is blocked");
    }

    private void OpenFolder()
    {
        _dataPath.Add(_localComponent.GetTextSafe(_displayedData[_cursor].data.fileName));
        FileExplorerFolderSO folder = (FileExplorerFolderSO)_displayedData[_cursor].data;

        SetDisplayData(folder.childs);
    }
    private void CloseFolder()
    {
        if (_dataPath.Count <= 0) return;

        _dataPath.RemoveAt(_dataPath.Count - 1);

        SetDisplayData(GetFolderByPath());
    }
    private List<FileExplorerDataSO> GetFolderByPath()
    {
        List<FileExplorerDataSO> currentList = _rootData;

        for (int i = 0; i < _dataPath.Count; i++)
        {
            string path = _dataPath[i];

            foreach (FileExplorerDataSO item in currentList)
            {
                if (item.fileName == path && item is FileExplorerFolderSO)
                {
                    currentList = ((FileExplorerFolderSO)item).childs;
                    break;
                }
            }
        }

        return currentList;
    }
}