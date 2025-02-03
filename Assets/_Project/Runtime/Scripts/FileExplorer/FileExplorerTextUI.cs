using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileExplorerTextUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;

    [Header("Parameters")]
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private Image _image;

    private Vector2 _scrollLimit;
    private int _nbLines = 0;
    private Vector2 _offset;
    private float _textSize;

    private void Start()
    {
        _scrollLimit.x = _rect.anchoredPosition.y;
        _scrollLimit.y = _scrollLimit.x;
    }

    public void SetTextValues(float textSize, Vector2 offset)
    {
        _title.fontSize = textSize;
        _mainText.fontSize = textSize;

        _title.rectTransform.sizeDelta = new Vector2(_title.rectTransform.sizeDelta.x, textSize);
        _title.rectTransform.anchoredPosition = new Vector2(offset.x, - (offset.y + (textSize/2f)));

        _mainText.rectTransform.anchoredPosition = new Vector2(offset.x, - (offset.y + (textSize * 1.5f)));
        _mainText.rectTransform.sizeDelta = new Vector2(256 - (2 * offset.x), _mainText.rectTransform.sizeDelta.y);

        _textSize = textSize;
        _offset = offset;
    }

    private void Update()
    {
        if (_nbLines != _mainText.textInfo.lineCount)
        {
            _image.rectTransform.anchoredPosition = new Vector2(_image.rectTransform.anchoredPosition.x, -(_offset.y + (_textSize * 3f)) - (_textSize * (_mainText.textInfo.lineCount + 1.5f)));
            _scrollLimit.y = _scrollLimit.x + (_offset.y + (_textSize * 2f) + _textSize * (_mainText.textInfo.lineCount - 1));
        }
    }

    public void SetInfos(string title, string text, Sprite sprite)
    {
        _title.text = title;

        if (text != null || text != "")
        {
            _mainText.text = text;
        }

        if (sprite != null)
        {
            _image.sprite = sprite;
        }
    }

    public void ScrollText(DIRECTIONAL_PAD_INFO direction)
    {
        switch (direction)
        {
            case DIRECTIONAL_PAD_INFO.UP:
                {
                    float temp = Mathf.Clamp(_rect.anchoredPosition.y - (_textSize * 2f), _scrollLimit.x, _scrollLimit.y);
                    _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, temp);
                    Debug.Log("UP : " + temp + " | " + _textSize);
                    break;
                }
            case DIRECTIONAL_PAD_INFO.DOWN:
                {
                    float temp = Mathf.Clamp(_rect.anchoredPosition.y + (_textSize * 2f), _scrollLimit.x, _scrollLimit.y);
                    _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, temp);
                    Debug.Log("DOWN : " + temp);
                    break;
                }

            default:
                return;
        }
    }
}
