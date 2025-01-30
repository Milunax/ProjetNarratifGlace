using System.Collections.Generic;
using UnityEngine;
using GMSpace;

public class FileExplorerBase : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] GameObject _textContainer;
    [SerializeField] FileExplorerLine _textLine;
    [Header("Display Paddings")]
    [SerializeField] Vector2 _topLeftPadding;
    [SerializeField] float _linePadding = 10f;
    [SerializeField] float _folderPadding = 20f;

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
        OpeningFileExplorer();
    }

    public void OpeningFileExplorer()
    {
        _dataForDisplay = new List<DISPLAYED_DATA>();

        for (int i = 0; i < _rootData.Count; i++)
        {
            SetDisplayData(_dataForDisplay, _rootData[i], i);
        }

        if (_dataForDisplay.Count <= 0) return;

        ResetCursorPos();
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
        List<int> browsCursor = new List<int>();
        browsCursor.Add(0);
        bool isEnd = false;

        while (!isEnd)
        {
            int cursorIndex = browsCursor.Count - 1;

            int currentPos = 0;
            foreach (int i in browsCursor)
            {
                currentPos += i;
            }
            if (currentPos >= startPos)
            {

            }

            

        }
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

            if (next >= GetDataCurrentListLength && GetDataCursorIndex > 0)
            {
                _dataCursor[GetDataCursorIndex -1] += 1;
                _dataCursor.RemoveAt(GetDataCursorIndex);
                return;
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

    private void DisplayLine(DISPLAYED_DATA data)
    {

    }
}
