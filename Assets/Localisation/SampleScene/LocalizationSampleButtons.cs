using LocalizationPackage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationSampleButtons : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Button1Text;
    [SerializeField] TextMeshProUGUI Button2Text;
    [SerializeField] TextMeshProUGUI Button3Text;
    [Space(10)]
    [SerializeField] Toggle CheckBox;

    Text test;
    private void OnEnable()
    {
        LocalizationManager.OnRefresh += RefreshTexts;
    }
    private void OnDisable()
    {
        LocalizationManager.OnRefresh -= RefreshTexts;
    }

    private void RefreshTexts(SystemLanguage currentLanguage)
    {
        Button1Text.text = LocalizationManager.Instance.GetLanguageForSelection(SystemLanguage.English, CheckBox.isOn);
        Button2Text.text = LocalizationManager.Instance.GetLanguageForSelection(SystemLanguage.French, CheckBox.isOn);
        Button3Text.text = LocalizationManager.Instance.GetLanguageForSelection(SystemLanguage.Spanish, CheckBox.isOn);
    }

    public void CheckBoxUpdate()
    {
        LocalizationManager.Instance.CallRefresh();
    }

    public void EnglishTextUpdate()
    {
        LocalizationManager.Instance.ChangeLanguage(SystemLanguage.English);
    }
    public void FrenchTextUpdate()
    {
        LocalizationManager.Instance.ChangeLanguage(SystemLanguage.French);
    }
    public void SpanishTextUpdate()
    {
        LocalizationManager.Instance.ChangeLanguage(SystemLanguage.Spanish);
    }
}
