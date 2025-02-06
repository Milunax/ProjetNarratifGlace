using UnityEngine;
using LocalizationPackage;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D;

public class MainMenu : MonoBehaviour
{
    [Header("Necessaries")]
    [SerializeField] private LocalizationComponent _locals;
    [SerializeField] private ScreenDisplay _screenDisplay;

    [Header("UI Elements")]
    [SerializeField] private Image _imagePlay;
    [SerializeField] private Image _imageQuit;
    [SerializeField] private Image _imageFrench;
    [SerializeField] private Image _imageEnglish;
    [SerializeField] private TextMeshProUGUI _textPlay;
    [SerializeField] private TextMeshProUGUI _textQuit;
    [SerializeField] private TextMeshProUGUI _textFrench;
    [SerializeField] private TextMeshProUGUI _textEnglish;

    [Header("UI Sprites")]
    [SerializeField] private Sprite _spriteSelected;
    [SerializeField] private Sprite _spriteUnselected;

    private int _cursor;

    private void OnEnable()
    {
        LocalizationManager.OnRefresh += Refresh;
        DirectionalPad.OnKeyPressed += Inputs;
    }
    private void OnDisable()
    {
        LocalizationManager.OnRefresh -= Refresh;
        DirectionalPad.OnKeyPressed -= Inputs;
    }

    private void Refresh(SystemLanguage language)
    {
        _textPlay.text = _locals.GetTextSafe("Menu_Play");
        _textQuit.text = _locals.GetTextSafe("Menu_Quit");

        _textFrench.text = LocalizationManager.Instance.GetLanguageForSelection(SystemLanguage.French, true);
        _textEnglish.text = LocalizationManager.Instance.GetLanguageForSelection(SystemLanguage.English, true);
    }

    private void Inputs(DIRECTIONAL_PAD_INFO infos)
    {
        switch (_cursor)
        {
            case 0:
                {
                    if (infos == DIRECTIONAL_PAD_INFO.UP)
                    {
                        _imageQuit.sprite = _spriteUnselected;
                        _cursor = 1;
                        _imagePlay.sprite = _spriteSelected;
                    }
                    else if (infos == DIRECTIONAL_PAD_INFO.CONFIRM) QuitGame();
                    
                    break;
                }
            case 1:
                {
                    if (infos == DIRECTIONAL_PAD_INFO.UP)
                    {
                        _imagePlay.sprite = _spriteUnselected;
                        _cursor = 2;
                        if (LocalizationManager.Instance.GetCurrentLanguage == "French")
                            _imageFrench.sprite = _spriteSelected;
                        else
                            _imageEnglish.sprite = _spriteSelected;
                    }
                    else if (infos == DIRECTIONAL_PAD_INFO.DOWN)
                    {
                        _imagePlay.sprite = _spriteUnselected;
                        _cursor = 0;
                        _imageQuit.sprite = _spriteSelected;
                    }
                    else if (infos == DIRECTIONAL_PAD_INFO.CONFIRM) PlayGame();
                    
                    break;
                }
            case 2:
                {
                    if ((LocalizationManager.Instance.GetCurrentLanguage == "French" && infos == DIRECTIONAL_PAD_INFO.LEFT) ||
                        (LocalizationManager.Instance.GetCurrentLanguage == "English" && infos == DIRECTIONAL_PAD_INFO.RIGHT))
                    {
                        SwitchLanguage();
                    }
                    else if (infos == DIRECTIONAL_PAD_INFO.DOWN)
                    {
                        _imageEnglish.sprite = _spriteUnselected;
                        _imageFrench.sprite = _spriteUnselected;
                        _cursor = 0;
                        _imagePlay.sprite = _spriteSelected;
                    }

                    break;
                }
        }
    }

    private void Start()
    {
        _imageQuit.sprite = _spriteUnselected;
        _imagePlay.sprite = _spriteSelected;
        _imageEnglish.sprite = _spriteUnselected;
        _imageFrench.sprite = _spriteUnselected;

        _cursor = 1;
    }

    public void PlayGame()
    {
        //_screenDisplay.Start
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SwitchLanguage()
    {
        switch (LocalizationManager.Instance.GetCurrentLanguage)
        {
            case "English":
                {
                    LocalizationManager.Instance.ChangeLanguage(SystemLanguage.French);

                    _imageEnglish.sprite = _spriteUnselected;
                    _imageFrench.sprite = _spriteSelected;
                    break;
                }
            case "French":
                {
                    LocalizationManager.Instance.ChangeLanguage(SystemLanguage.English);

                    _imageEnglish.sprite = _spriteSelected;
                    _imageFrench.sprite = _spriteUnselected;
                    break;
                }
        }
    }
}
