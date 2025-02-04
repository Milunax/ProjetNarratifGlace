using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class FileExplorerTextUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;

    [Header("Parameters")]
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private Image _image;
    [SerializeField] private Image _logo;

    private Vector2 _scrollLimit;
    private int _nbLines = -1;
    private Vector2 _offset;
    private float _textSize;
    private Vector2 _imageSize;

    private void Awake()
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

        _logo.rectTransform.anchoredPosition = new Vector2(offset.x, 0);

        _textSize = textSize;
        _offset = offset;
        _imageSize = _image.rectTransform.sizeDelta;
    }

    private void Update()
    {
        if (_nbLines != _mainText.textInfo.lineCount )
        {
            _image.rectTransform.anchoredPosition = new Vector2(_image.rectTransform.anchoredPosition.x, -(_offset.y + (_textSize * 3f)) - ((_textSize * 1.125f) * _mainText.textInfo.lineCount));
            _logo.rectTransform.anchoredPosition = new Vector2(_logo.rectTransform.anchoredPosition.x, -(_offset.y + (_textSize * 3f)) - ((_textSize * 1.125f) * (_mainText.textInfo.lineCount + 1)) - _image.rectTransform.sizeDelta.y);

            if (_image.sprite != null)
            {
                _scrollLimit.y = _scrollLimit.x + (_offset.y + (_textSize * 2f) + ((_textSize * 1.125f) * (_mainText.textInfo.lineCount - 1)) + (_image.rectTransform.sizeDelta.y / 4));
            }
            else
            {
                _scrollLimit.y = _scrollLimit.x + (_offset.y + (_textSize * 2f) + ((_textSize * 1.125f) * (_mainText.textInfo.lineCount - 1)) - (_imageSize.y * 0.7f));
            }
        }

        _nbLines = _mainText.textInfo.lineCount;
    }

    public void SetInfos(string title, string text, Sprite sprite)
    {
        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, _scrollLimit.x);

        _title.text = title;

        if (text != null || text != "")
        {
            _mainText.text = text;
        }

        if (sprite != null)
        {
            _image.sprite = sprite;
            _image.rectTransform.sizeDelta = _imageSize;
        }
        else
        {
            _image.sprite = null;
            _image.rectTransform.sizeDelta = Vector2.zero;
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
                    break;
                }
            case DIRECTIONAL_PAD_INFO.DOWN:
                {
                    float temp = Mathf.Clamp(_rect.anchoredPosition.y + (_textSize * 2f), _scrollLimit.x, _scrollLimit.y);
                    _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, temp);
                    break;
                }

            default:
                return;
        }
    }
}
