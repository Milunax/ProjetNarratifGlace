using System.Collections.Generic;
using UnityEngine;
using GMSpace;
using UnityEngine.UI;

public class FileExplorerBase : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private GameObject _textContainer;
    private RectTransform _rectTextContainer;
    [SerializeField] private GameObject _textLinePrefab;
    private List<FileExplorerLine> _textLines = new List<FileExplorerLine>();

    private float GetTextContainerSize { get => _rectTextContainer.sizeDelta.x; }

    [Header("Display Paddings")]
    [SerializeField] private Vector2 _topLeftPadding;
    [SerializeField] private float _linePadding = 10f;
    [SerializeField] private float _folderPadding = 20f;
    [SerializeField] private float _textSize = 20f;

    [Header("Display Scroll")]
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


    private List<DISPLAYED_DATA> _dataForDisplay = new List<DISPLAYED_DATA>();
    private List<int> _dataCursor = new List<int>();
    private int GetDataCursorIndex { get => _dataCursor.Count-1; } 
    private List<DISPLAYED_DATA> GetDataCurrentList
    {
        get
        {
            List<DISPLAYED_DATA> current = _dataForDisplay;

            for (int i = 0; i < _dataCursor.Count - 1; i++)
            {
                current = current[i].childs;
            }

            return current;
        }
    }
    private int GetDataCurrentListLength
    {
        get
        {
            List<DISPLAYED_DATA> current = _dataForDisplay;

            for (int i = 0;  i < _dataCursor.Count - 1; i++)
            {
                current = current[i].childs;
            }

            return current.Count;
        }
    }
    private DISPLAYED_DATA GetDataCurrentItem
    {
        get
        {
            List<DISPLAYED_DATA> current = _dataForDisplay;

            for (int i = 0; i < _dataCursor.Count - 1; i++)
            {
                current = current[i].childs;
            }

            return current[_dataCursor[GetDataCursorIndex]];
        }
    }
    private FileExplorerDataSO GetDataCurrentRaw
    {
        get
        {
            List<DISPLAYED_DATA> currentDisplay = _dataForDisplay;
            List<FileExplorerDataSO> currentData = _rootData;

            for (int i = 0; i < _dataCursor.Count - 1; i++)
            {
                currentData = ((FileExplorerFolderSO)currentData[currentDisplay[_dataCursor[i]].indexList]).childs;
                currentDisplay = currentDisplay[i].childs;
            }

            return currentData[currentDisplay[_dataCursor[GetDataCursorIndex]].indexList];
        }
    }

    private struct DISPLAYED_DATA
    {
        public string displayName;
        public FILE_TYPE fileType;
        public int indexList;
        public bool isBlocked;
        public List<DISPLAYED_DATA> childs;

        public DISPLAYED_DATA(string nName, FILE_TYPE nFileType, int nIndex, bool nBlocked)
        {
            displayName = nName;
            fileType = nFileType;
            indexList = nIndex;
            isBlocked = nBlocked;
            childs = new List<DISPLAYED_DATA>();
        }
    }

    private void Start()
    {
        TryGetComponent<RectTransform>(out _rectTextContainer);
        SetupTextDisplayValues();

        Opening();
    }

    public void Opening()
    {
        _dataForDisplay = new List<DISPLAYED_DATA>();

        for (int i = 0; i < _rootData.Count; i++)
        {
            SetDisplayData(_dataForDisplay, _rootData[i], i);
        }

        if (_dataForDisplay.Count <= 0) return;

        ResetCursorPos();

        BrowsDataRefresh();
    }

    private void SetDisplayData(List<DISPLAYED_DATA> dataList, FileExplorerDataSO current, int index)
    {
        if (current.isVisible.day < GameManager.Instance.GetSetProgressionDay &&
            current.isVisible.inDay < GameManager.Instance.GetSetProgressionInDay &&
            current.isVisible.narrativeIALevel < GameManager.Instance.GetSetNarrativeIALevel) return; // if file/folder isn't visible

        FILE_TYPE type = GetFileType(current);

        if (current.isAccessible.day >= GameManager.Instance.GetSetProgressionDay &&
            current.isAccessible.inDay >= GameManager.Instance.GetSetProgressionInDay &&
            current.isAccessible.narrativeIALevel >= GameManager.Instance.GetSetNarrativeIALevel) // if file/folder is accessible
        {
            if (type == FILE_TYPE.FOLDER) // if folder
            {
                FileExplorerFolderSO folder = (FileExplorerFolderSO)current;
                foreach (FileExplorerDataSO file in folder.childs)
                {
                    if (file == null) folder.childs.Remove(file); // if file in folder is empty, remove it
                }

                if (folder.childs.Count <= 0) { return; } // if folder is empty, don't display
            }

            dataList.Add(new DISPLAYED_DATA(current.fileName, type, index, false));
        }
        else // if file/folder isn't accessible
        {
            dataList.Add(new DISPLAYED_DATA(current.fileName, type, index, true));
        }
    }

    private FILE_TYPE GetFileType(FileExplorerDataSO file)
    {
        if (file is FileExplorerFolderSO) return FILE_TYPE.FOLDER;
        if (file is FileExplorerFileTextSO) return FILE_TYPE.FILE_TEXT;
        if (file is FileExplorerAudioFileSO) return FILE_TYPE.FILE_AUDIO;
        return FILE_TYPE.NULL;
    }

    private void BrowsDataRefresh(int startPos = 0)
    {
        if (startPos < 0) startPos = 0;

        List<int> browsCursor = new List<int>();
        browsCursor.Add(0);
        bool isEnd = false;

        if (_textLines.Count-1 >= startPos)
        {
            for (int i = startPos; i < _textLines.Count; i++)
            {
                FileExplorerLine item = _textLines[i];
                _textLines.Remove(item);
                item.Delete();
            }
        }

        while (!isEnd)
        {
            int cursorIndex = browsCursor.Count - 1;

            int currentLine = TotalDataLines(browsCursor);

            List<DISPLAYED_DATA> currentList = _dataForDisplay;

            for (int i = 0; i < browsCursor.Count - 1; i++)
            {
                currentList = currentList[i].childs;
            }
            DISPLAYED_DATA currentItem = currentList[browsCursor[cursorIndex]];

            if (currentLine >= startPos)
            {
                FileExplorerLine item = CreateTextLine(currentItem);
                SetDisplayLines(item, currentLine, browsCursor.Count - 1);
            }

            if (currentItem.childs.Count > 0)
            {
                _dataCursor.Add(0);
            }
            else
            {
                int next = browsCursor[cursorIndex] + 1;

                if (next < currentList.Count)
                {
                    browsCursor[cursorIndex] = next;
                }
                else
                {
                    for (int i = 0; i < _dataCursor.Count; i++)
                    {
                        if (next >= currentList.Count && cursorIndex > 0)
                        {
                            browsCursor.RemoveAt(cursorIndex);
                            cursorIndex = browsCursor.Count - 1;
                            next = _dataCursor[cursorIndex] + 1;
                        }
                        else
                        {
                            if (next < currentList.Count) _dataCursor[cursorIndex] = next;
                            else { isEnd = true; }
                        }
                    }
                }
            }
        }

        UpdateDisplayLine();
    }

    private void ResetCursorPos()
    {
        _dataCursor.Clear();
        _dataCursor.Add(0);
    }
    private void MoveCursor(bool tempo/*given movement input (UP/DOWN)*/)
    {
        if (tempo) //move up
        {
            int previous = _dataCursor[GetDataCursorIndex] -1;

            if (previous >= 0)
            {
                _dataCursor[GetDataCursorIndex] = previous;

                if (GetDataCurrentItem.childs.Count > 0)
                {
                    _dataCursor.Add(GetDataCurrentItem.childs.Count -1);
                }

                return;
            }

            if (previous < 0 && GetDataCursorIndex > 0)
            {
                _dataCursor.RemoveAt(GetDataCursorIndex);
                return;
            }
        }
        else if (!tempo) //move down
        {
            if (GetDataCurrentItem.childs.Count > 0)
            {
                _dataCursor.Add(0);
                return;
            }

            int next = _dataCursor[GetDataCursorIndex] + 1;

            if (next < GetDataCurrentListLength)
            {
                _dataCursor[GetDataCursorIndex] = next;
                return;
            }

            for (int i = 0; i < _dataCursor.Count; i++)
            {
                if (next >= GetDataCurrentListLength && GetDataCursorIndex > 0)
                {
                    _dataCursor.RemoveAt(GetDataCursorIndex);
                    next = _dataCursor[GetDataCursorIndex] + 1;
                }
                else
                {
                    if (next < GetDataCurrentListLength) _dataCursor[GetDataCursorIndex] = next;
                    return;
                }
            }

        }
    }

    private void OpenFolder()
    {
        FileExplorerFolderSO data = (FileExplorerFolderSO)GetDataCurrentRaw;
        List<DISPLAYED_DATA> currentDisplay = GetDataCurrentList;

        for (int i = 0; i < data.childs.Count; i++)
        {
            SetDisplayData(currentDisplay[_dataCursor[GetDataCursorIndex]].childs, data.childs[i], i);
        }

        _dataCursor.Add(0);
    }

    private int TotalDataLines(int level = 0)
    {
        int total = 0;

        foreach (DISPLAYED_DATA item in _dataForDisplay)
        {
            if (item.childs.Count > 0) total += TotalDataLines(level + 1);
            total++;
        }

        return total;
    }
    private int TotalDataLines(List<int> list, int level = 0)
    {
        int total = 0;

        int i = 0;
        foreach (DISPLAYED_DATA item in _dataForDisplay)
        {
            if (level < list.Count && list[level] < i) break;

            if (item.childs.Count > 0) total += TotalDataLines(list, level + 1);
            total++;
        }

        return total;
    }

    private FileExplorerLine CreateTextLine(DISPLAYED_DATA data)
    {
        GameObject line = Instantiate(_textLinePrefab, _textContainer.transform);
        line.name = data.displayName;

        line.TryGetComponent<FileExplorerLine>(out FileExplorerLine scriptLine);
        _textLines.Add(scriptLine);

        scriptLine.SetupVisual(_rectTextContainer.sizeDelta.x, _textSize, data.displayName, data.fileType, data.isBlocked);

        return scriptLine;
    }
    private void SetupTextDisplayValues()
    {
        _topPosY = -(_topLeftPadding.y + (_textSize / 2));
        _bottomPosY = -GetTextContainerSize - _topPosY;

        int count = 0;
        float temp = _topPosY;
        while (temp >= _bottomPosY)
        {
            count++;
            temp -= _linePadding + _textSize;
        }

        _maxLineOnScreen = count;
    }
    private void SetDisplayLines(FileExplorerLine item, int line, int column)
    {
        item.GetRectTransform.localPosition = new Vector2(item.GetRectTransform.localPosition.x + (_folderPadding * column),
                                                          item.GetRectTransform.localPosition.y - ((_linePadding + _textSize) * line));

        if (line >= _skippedSteps && line <= _maxLineOnScreen) item.SetVisibility(true);
        else item.SetVisibility(false);
    }
    private void UpdateDisplayLine()
    {
        int halfMaxLine = _maxLineOnScreen / 2;

        int currentIndex = TotalDataLines(_dataCursor);
        if (currentIndex >= halfMaxLine && currentIndex <= _textLines.Count - halfMaxLine)
        {
            _skippedSteps = currentIndex - halfMaxLine;
        }

        _rectTextContainer.localPosition = new Vector2(_rectTextContainer.localPosition.x, (_linePadding + _textSize) * _skippedSteps);

        for (int i = 0; i < _textLines.Count; i++)
        {
            if (i < _skippedSteps) _textLines[i].SetVisibility(false);
            else if (i >= _skippedSteps && i <= _skippedSteps + _maxLineOnScreen) _textLines[i].SetVisibility(true);
            else _textLines[i].SetVisibility(false);
        }
    }
}
