using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileExplorerLine : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshPro;
    [SerializeField] Image _imageIcon;
    [SerializeField] Image _imageCursor;

    [Header("Initialisation")]
    [SerializeField] float _textSize = 36f;
    [SerializeField] float _widthSpacing = 5f;

    [Header("Textures")]
    [SerializeField] Sprite _cursor;
    [Space(5)]
    [SerializeField] Sprite _folderAcc;
    [SerializeField] Sprite _folderBlc;
    [Space(5)]
    [SerializeField] Sprite _TextFileAcc;
    [SerializeField] Sprite _TextFileBlc;
    [Space(5)]
    [SerializeField] Sprite _AudioFileAcc;
    [SerializeField] Sprite _AudioFileBlc;

    private void Start()
    {
        SetupVisual(256, 256, "Lorem Ipsum");
    }
    public void SetupVisual(float boxWidth, float boxHeight, string text)
    {
        _textMeshPro.fontSize = _textSize;
        _textMeshPro.rectTransform.sizeDelta = new Vector2(boxWidth, _textSize);

        _imageIcon.rectTransform.sizeDelta = new Vector2(_textSize, _textSize);
        _imageCursor.rectTransform.sizeDelta = new Vector2 (_textSize, _textSize);

        _imageIcon.rectTransform.localPosition = new Vector2(0, 0);
        _imageCursor.rectTransform.localPosition = new Vector2(-(_textSize + _widthSpacing), 0);
        _textMeshPro.rectTransform.localPosition = new Vector2(boxWidth/2f +  _textSize/2f + _widthSpacing, 0);

        _textMeshPro.text = text;
    }

    public void ShowCursor(bool value)
    {
        _imageCursor.enabled = value;
    }
}
