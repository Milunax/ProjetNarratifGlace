using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileExplorerLine : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;
    public RectTransform GetRectTransform { get => _rect; }

    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private Image _imageIcon;
    [SerializeField] private Image _imageCursor;
    public bool GetSetCursor
    {
        get => _imageCursor.enabled;
        set => _imageCursor.enabled = value;
    }

    [Header("Initials")]
    [InfoBox("Based on a size of text of 20")]
    [SerializeField] private float _widthSpacing = 2f;
    private bool _isOnScreen = false;
    public bool GetIsOnScreen { get => _isOnScreen; }

    [Header("Textures")]
    [SerializeField] private Sprite _cursor;
    [Space(5)]
    [SerializeField] private Sprite _folderAcc;
    [SerializeField] private Sprite _folderBlc;
    [Space(5)]
    [SerializeField] private Sprite _TextFileAcc;
    [SerializeField] private Sprite _TextFileBlc;
    [Space(5)]
    [SerializeField] private Sprite _AudioFileAcc;
    [SerializeField] private Sprite _AudioFileBlc;

    private void Start()
    {
        //SetupVisual(256, 36, "Lorem Ipsum", FILE_TYPE.FILE_TEXT, false);
    }
    public void SetupVisual(float boxWidth, float textSize, string text, FILE_TYPE type, bool blocked)
    {
        float actualSpacing = (textSize / 20f) * _widthSpacing;

        _textMeshPro.fontSize = textSize;
        _textMeshPro.rectTransform.sizeDelta = new Vector2(boxWidth, textSize);

        _imageIcon.rectTransform.localScale = new Vector2(textSize / _imageIcon.rectTransform.sizeDelta.x, textSize/_imageIcon.rectTransform.sizeDelta.y);
        _imageCursor.rectTransform.localScale = new Vector2 (textSize / _imageCursor.rectTransform.sizeDelta.x, textSize / _imageCursor.rectTransform.sizeDelta.y);

        _imageIcon.rectTransform.localPosition = new Vector2(0, 0);
        _imageCursor.rectTransform.localPosition = new Vector2(-(textSize + actualSpacing), 0);
        _textMeshPro.rectTransform.localPosition = new Vector2(boxWidth/2f +  textSize/2f + actualSpacing, 0);

        _textMeshPro.text = text;

        switch (type)
        {
            case FILE_TYPE.FOLDER:
                if (blocked) _imageIcon.sprite = _folderBlc;
                else _imageIcon.sprite = _folderAcc;
                break;

            case FILE_TYPE.FILE_TEXT:
                if (blocked) _imageIcon.sprite = _TextFileBlc;
                else _imageIcon.sprite = _TextFileAcc;
                break;

            case FILE_TYPE.FILE_AUDIO:
                if (blocked) _imageIcon.sprite = _AudioFileBlc;
                else _imageIcon.sprite = _AudioFileAcc;
                break;

            case FILE_TYPE.NULL:
            default:
                _imageIcon.enabled = false;
                break;
        }

        GetSetCursor = false;
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    public void SetVisibility(bool visible)
    {
        _textMeshPro.enabled = visible;
        _imageIcon.enabled = visible;
        _textMeshPro.enabled = visible;

        _isOnScreen = visible;
    }
}
