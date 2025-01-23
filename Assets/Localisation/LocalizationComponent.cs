using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LocalizationPackage;

public class LocalizationComponent : MonoBehaviour
{
    [SerializeField] TextAsset TSVFile;
    //[SerializeField] bool useTSVFile = true;
    [Space(10)]
    //[SerializeField] List<TextInEditor> textInEditor;

    Dictionary<string, string[]> MapOfTexts = new Dictionary<string, string[]>();
    bool cantBeUse = true;
    bool endInit = false;

    public bool GetEndInit { get => endInit; }

    [System.Serializable]
    public struct TextInEditor
    {
        public string Key;
        public string[] Texts;
    }

    /////////////
    // METHODS //
    /////////////

    private void Start()
    {
        //if (LocalizationManager.Instance.gameObject.name == gameObject.name) useTSVFile = true;

        //if (useTSVFile) ReadTSVFile();
        //else TextEditorIntoMap();

        ReadTSVFile();

        endInit = true;
    }

    //private void TextEditorIntoMap()
    //{
    //    MapOfTexts = new Dictionary<string, string[]>();

    //    for (int i = 0; i < textInEditor.Count; i++)
    //    {
    //        MapOfTexts.Add(textInEditor[0].Key, textInEditor[0].Texts);
    //    }

    //    if (!MapOfTexts.ContainsKey("Languages"))
    //    {
    //        Debug.LogError("'" + gameObject.name + "' list does not contains 'Languages' key, check if list does contain 'Languages' as a key in the first line ", gameObject);
    //        return;
    //    }

    //    if (MapOfTexts["Languages"].Length <= 1 || IndexDefaultLanguage() <= -1) // Does File contains any usable Data
    //    {
    //        Debug.LogError("Unreadable data in '" + gameObject.name + "', check if languages are correclty indicated on the first line of the file and doesn't have any typo ", gameObject);
    //        return;
    //    }

    //    cantBeUse = false;
    //}
    void ReadTSVFile()
    {
        if (TSVFile == null) // Is filePath empty
        {
            Debug.LogWarning("No referenced file in '" + gameObject.name + "', check if the file is correctly set. ", gameObject);
            return;
        }


        string[] lines = TSVFile.text.Split('\n');
        if (lines == null) // Is file empty
        {
            Debug.LogError("No Data in file in '" + gameObject.name + "', check if file is empty. ", gameObject);
            return;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split('\t');

            if (i == 0)
            {
                MapOfTexts.Add("Languages", columns);
            }
            else
                MapOfTexts.Add(columns[0], columns);
        }

        if (MapOfTexts["Languages"].Length <= 1 || IndexDefaultLanguage() <= -1) // Does File contains any usable Data
        {
            Debug.LogError("Unreadable data in '" + gameObject.name + "', check if languages are correclty indicated on the first line of the file and doesn't have any typo ", gameObject);
            return;
        }

        cantBeUse = false;
    }

    private string ErrorCall()
    {
        Debug.LogError("Fatal error in initial parameters of '" + gameObject.name + "', see logs for more information ", gameObject);
        return "ERROR SEE LOGS";
    }

    private int IndexCurrentLanguage()
    {
        string[] listOfLanguagues = MapOfTexts["Languages"];
        for (int i = 1; i < listOfLanguagues.Length; i++)
        {
            if (listOfLanguagues[i].Contains(LocalizationManager.Instance.GetCurrentLanguage)) { return i; }
        }

        return -1;
    }
    private int IndexDefaultLanguage()
    {
        string[] listOfLanguagues = MapOfTexts["Languages"];
        for (int i = 1; i < listOfLanguagues.Length; i++)
        {
            if (listOfLanguagues[i].Contains(LocalizationManager.Instance.GetDefaultLanguage)) { return i; }
        }

        return -1;
    }
    private int IndexGivenLanguage(string language)
    {
        string[] listOfLanguagues = MapOfTexts["Languages"];
        for (int i = 1; i < listOfLanguagues.Length; i++)
        {
            if (listOfLanguagues[i].Contains(language)) { return i; }
        }

        return -1;
    }

    public string GetText(string Key, bool keyIsLanguage = false)
    {
        if (cantBeUse) return ErrorCall();

        if (!MapOfTexts.ContainsKey(Key)) 
        { 
            Debug.LogError("No Associated key in '" + gameObject.name + "' with key: " + Key); 
            return "NO ASSOCIATED KEY"; 
        }

        int index;
        if (keyIsLanguage)
            index = IndexGivenLanguage(Key);
        else
            index = IndexCurrentLanguage();

        if (index <= -1)
        {
            index = IndexDefaultLanguage();

            if (index <= -1)
            {
                Debug.LogError(LocalizationManager.Instance.GetDefaultLanguage + ", as default language, isn't supported in '" + gameObject.name + "' .tsv file, check if said language is correclty indicated ", gameObject);
                return "DEFAULT LANGUAGE NOT SUPPORTED";
            }
            else
                Debug.LogWarning(LocalizationManager.Instance.GetCurrentLanguage + " isn't supported in '" + gameObject.name + "' .tsv file, check if said language is correclty indicated ", gameObject);
        }

        string text = MapOfTexts[Key][index];
        if (text == "")
        {
            index = IndexDefaultLanguage();
            text = MapOfTexts[Key][index];

            if (text == "")
            {
                Debug.LogError("Text is empty for " + Key + " key in current & default language in '" + gameObject.name + "' .tsv file, check is said text is correctly set in file ", gameObject);
                return "TEXT IS NULL";
            }
            else
            {
                Debug.LogWarning(LocalizationManager.Instance.GetCurrentLanguage + " text is empty for " + Key + " key in '" + gameObject.name + "' .tsv file, text returned by default in " + LocalizationManager.Instance.GetDefaultLanguage + " ", gameObject);
            }
        }

        return text;
    }
}
