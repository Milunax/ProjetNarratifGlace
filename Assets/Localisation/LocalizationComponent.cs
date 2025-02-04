using System.Collections.Generic;
using UnityEngine;
using LocalizationPackage;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LocalizationComponent : MonoBehaviour
{
    [SerializeField] TextAsset TSVFile;

    [SerializeField] List<TextMeshProUGUI> TMPList;
    [SerializeField] List<Text> TextList;
    [SerializeField] List<TextMesh> TextMeshList;

    [SerializeField] List<KeyToText> TextsKeys = new List<KeyToText>();

    Dictionary<string, string[]> MapOfTexts = new Dictionary<string, string[]>();

    bool cantBeUse = true;
    bool isManager = false;
    bool endInit = false;

    public bool GetEndInit { get => endInit; }
    public int GetNbTextAssets { get => TMPList.Count + TextList.Count + TextMeshList.Count; }
    public string GetTSVFileName { get => TSVFile.name; }

    /////////////
    // METHODS //
    /////////////


    private void OnValidate()
    {
        isManager = gameObject.TryGetComponent<LocalizationManager>(out LocalizationManager manager);
    }

    private void OnEnable()
    {
        LocalizationManager.OnRefresh += RefreshItems;
    }
    private void OnDisable()
    {
        LocalizationManager.OnRefresh += RefreshItems;
    }

    private void Start()
    {
        isManager = gameObject.TryGetComponent<LocalizationManager>(out LocalizationManager manager);

        ReadTSVFile();

        endInit = true;
    }

    private void ReadTSVFile()
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
            {
                MapOfTexts.Add(CleanKey(columns[0]), columns);
            }
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
    /// <summary>
    /// Give a text depending on the TSV file present in the class, the keys provided in parameter, and the current game language defined in the LocalizationManager.
    /// </summary>
    /// <param name="Key">Given identification key corresponding to a text in the TSV file. Can contain multiple key if separated by "&&"</param>
    /// <returns>Text in current game language. Will return text on multiple lines if multiple keys are given</returns>
    public string GetTextSafe(string Key)
    {
        if (Key == null)
        {
            Debug.LogError("Key was null", gameObject);
            return "KEY WAS NULL";
        }

        Key = CleanKey(Key);

        string result = "";
        string[] tempo = Key.Split("&&");
        foreach (string key in tempo)
        {
            if (result != "") result += '\n';
            result += GetTextUnsafe(key);
        }

        return result;
    }

    /// <summary>
    /// Give a text depending on the TSV file present in the class, the key provided in parameter, and the current game language defined in the LocalizationManager.
    /// </summary>
    /// <param name="Key">Given identification key corresponding to a text in the TSV file.</param>
    /// <param name="keyIsLanguage">Intended for LocalizationManager use only, keep at "false" by default to avoid unexpected errors.</param>
    /// <returns>Text in current game language.</returns>
    public string GetTextUnsafe(string Key, bool keyIsLanguage = false)
    {
        if (cantBeUse) return ErrorCall();
        if (Key == null)
        {
            Debug.LogError("Key was null", gameObject);
            return "KEY WAS NULL";
        }
        if (Key == "")
        {
            Debug.LogWarning("Key was empty", gameObject);
            return "";
        }
        if (!MapOfTexts.ContainsKey(Key)) 
        { 
            Debug.LogError("No Associated key in '" + TSVFile.name + "' with key: " + Key, gameObject);
            return "NO ASSOCIATED KEY : '" + Key + "' in '" + TSVFile.name + "' "; 
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
                Debug.LogError(LocalizationManager.Instance.GetDefaultLanguage + ", as default language, isn't supported in '" + TSVFile.name + "' .tsv file, check if said language is correclty indicated ", gameObject);
                return "DEFAULT LANGUAGE NOT SUPPORTED : '" + LocalizationManager.Instance.GetDefaultLanguage + "' in '" + TSVFile.name + "' ";
            }
            else
            {
                Debug.LogWarning(LocalizationManager.Instance.GetCurrentLanguage + " isn't supported in '" + TSVFile.name + "' .tsv file, check if said language is correclty indicated ", gameObject);
            }
        }


        string text = "";
        if (index < MapOfTexts[Key].Length)
            text = MapOfTexts[Key][index];

        if (text == "")
        {
            index = IndexDefaultLanguage();
            if (index < MapOfTexts[Key].Length)
                text = MapOfTexts[Key][index];

            if (text == "")
            {
                Debug.LogError("Text is empty for " + Key + " key in current & default language in '" + gameObject.name + "' .tsv file, check is said text is correctly set in file ", gameObject);
                return "TEXT IS NULL : '" + LocalizationManager.Instance.GetDefaultLanguage + "' at '" + Key + "' key in '" + TSVFile.name + "' ";
            }
            else
            {
                Debug.LogWarning(LocalizationManager.Instance.GetCurrentLanguage + " text is empty for " + Key + " key in '" + gameObject.name + "' .tsv file ", gameObject);
                if (!LocalizationManager.Instance.DebugGetReturnDefault) return "TEXT IS NULL : '" + LocalizationManager.Instance.GetCurrentLanguage + "' at '" + Key + "' key in '" + TSVFile.name + "' ";
            }
        }

        return text;
    }

    private void RefreshItems(SystemLanguage language)
    {

        if (TMPList.Count > 0)
        {

            for (int i = 0; i < TMPList.Count; i++)
            {
                TextsKeys[KeyToText.GetStructIndex(ref TextsKeys, "TMP" + i.ToString())].Value = CleanKey(KeyToText.GetValue(ref TextsKeys, "TMP" + i.ToString()));

                if (KeyToText.GetValue(ref TextsKeys, "TMP" + i.ToString()) == "")
                {
                    Debug.LogWarning("Key value is empty for '" + TMPList[i].gameObject.name + "', pls check if key value is correclty set", gameObject);
                    continue;
                }

                string result = "";
                string[] tempo = KeyToText.GetValue(ref TextsKeys, "TMP" + i.ToString()).Split("&&");
                foreach (string key in tempo)
                {
                    if (result != "") result += '\n';
                    result += GetTextUnsafe(key);
                }

                TMPList[i].text = result;
            }
        }

        if (TextList.Count > 0)
        {
            for (int i = 0; i < TextList.Count; i++)
            {
                TextsKeys[KeyToText.GetStructIndex(ref TextsKeys, "Text" + i.ToString())].Value = CleanKey(KeyToText.GetValue(ref TextsKeys, "Text" + i.ToString()));

                if (KeyToText.GetValue(ref TextsKeys, "Text" + i.ToString()) == "")
                {
                    Debug.LogWarning("Key value is empty for '" + TextList[i].gameObject.name + "', pls check if key value is correclty set", gameObject);
                    continue;
                }

                string result = "";
                string[] tempo = KeyToText.GetValue(ref TextsKeys, "Text" + i.ToString()).Split("&&");
                foreach (string key in tempo)
                {
                    if (result != "") result += '\n';
                    result += GetTextUnsafe(key);
                }

                TextList[i].text = result;
            }
        }


        if (TextMeshList.Count > 0)
        {
            for (int i = 0; i < TextMeshList.Count; i++)
            {
                TextsKeys[KeyToText.GetStructIndex(ref TextsKeys, "TextMesh" + i.ToString())].Value = CleanKey(KeyToText.GetValue(ref TextsKeys, "TextMesh" + i.ToString()));

                if (KeyToText.GetValue(ref TextsKeys, "TextMesh" + i.ToString()) == "")
                {
                    Debug.LogWarning("Key value is empty for '" + TextMeshList[i].gameObject.name + "', pls check if key value is correclty set", gameObject);
                    continue;
                }

                string result = "";
                string[] tempo = KeyToText.GetValue(ref TextsKeys, "TextMesh" + i.ToString()).Split("&&");
                foreach (string key in tempo)
                {
                    if (result != "") result += '\n';
                    result += GetTextUnsafe(key);
                }

                TextMeshList[i].text = result;
            }
        }
    }
    private string CleanKey(string key, bool superClean = false)
    {
        string result = key.Replace(" ", "").Replace("\t", "");
        return result;
    }

    private bool RemoveItem<T>(T item)
    {
        int index = -1;

        if (item is TextMeshProUGUI) {
            TextMeshProUGUI itemTMP = item as TextMeshProUGUI;
            index = TMPList.IndexOf(itemTMP);
            if (index < 0) return false;

            for (int i = index; i < TMPList.Count-1; i++)
            {
                TextsKeys[KeyToText.GetStructIndex(ref TextsKeys, "TMP" + index.ToString())].Value = KeyToText.GetValue(ref TextsKeys, "TMP" + (index+1).ToString());
            }

            KeyToText.RemoveItem(ref TextsKeys, "TMP" + (TMPList.Count - 1).ToString());
            TMPList.Remove(itemTMP);
        }
        else if (item is Text)
        {
            Text itemText = item as Text;
            index = TextList.IndexOf(itemText);
            if (index < 0) return false;

            for (int i = index; i < TextList.Count - 1; i++)
            {
                TextsKeys[KeyToText.GetStructIndex(ref TextsKeys, "Text" + index.ToString())].Value = KeyToText.GetValue(ref TextsKeys, "Text" + (index + 1).ToString());
            }

            KeyToText.RemoveItem(ref TextsKeys, "Text" + (TMPList.Count - 1).ToString());
            TextList.Remove(itemText);
        }
        else if (item is TextMesh)
        {
            TextMesh itemTextMesh = item as TextMesh;
            index = TextMeshList.IndexOf(itemTextMesh);
            if (index < 0) return false;

            for (int i = index; i < TextMeshList.Count - 1; i++)
            {
                TextsKeys[KeyToText.GetStructIndex(ref TextsKeys, "TextMesh" + index.ToString())].Value = KeyToText.GetValue(ref TextsKeys, "TextMesh" + (index + 1).ToString());
            }

            KeyToText.RemoveItem(ref TextsKeys, "TextMesh" + (TMPList.Count - 1).ToString());
            TextMeshList.Remove(itemTextMesh);
        }
        else { return false; }

        return true;
    }

    /////////////
    /// CLASS ///
    /////////////
    [System.Serializable]
    public class KeyToText
    {
        [SerializeField] public string Key;
        [SerializeField] public string Value;

        public static bool ContainsKey(ref List<KeyToText> list, string key)
        {
            foreach(KeyToText take in list)
            {
                if (take.Key == key) return true;
            }
            return false;
        }

        public static string GetValue(ref List<KeyToText> list, string key)
        {
            foreach (KeyToText take in list)
            {
                if (take.Key == key) return take.Value;
            }
            return null;
        }

        public static int GetStructIndex(ref List<KeyToText> list, string key)
        {
            int i = 0;
            foreach (KeyToText take in list)
            {
                if (take.Key == key) return i;
                i++;
            }

            return -1;
        }

        public static bool RemoveItem(ref List<KeyToText> list, string key)
        {
            foreach (KeyToText take in list)
            {
                if (take.Key == key)
                {
                    list.Remove(take);
                    return true;
                }
            }
            return false;
        }

        public static bool AddItem(ref List<KeyToText> list, string key, string value)
        {
            if (ContainsKey(ref list, key)) return false;

            KeyToText item = new KeyToText();
            item.Key = key;
            item.Value = value;

            list.Add(item);

            return true;
        }
    }

    /////////////////////
    /// CUSTOM EDITOR ///
    /////////////////////

#if UNITY_EDITOR
    [CustomEditor(typeof(LocalizationComponent))]
    public class LocalizationEditor : Editor
    {
        LocalizationComponent langComp;

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            langComp = (LocalizationComponent)target;

            if (langComp.isManager)
            {
                if (langComp.TSVFile) Undo.RecordObject(langComp.TSVFile, "Change TSV File");
                langComp.TSVFile = EditorGUILayout.ObjectField("Language Selection TSV File", langComp.TSVFile, typeof(TextAsset), true) as TextAsset;
            }
            else
            {
                if (langComp.TSVFile) Undo.RecordObject(langComp.TSVFile, "Change TSV File");
                langComp.TSVFile = EditorGUILayout.ObjectField("TSV File", langComp.TSVFile, typeof(TextAsset), true) as TextAsset;

                if (GUILayout.Button("Get Text Assets In Scene"))
                {
                    LocalizationWindow window = LocalizationWindow.ShowWindow();
                    window.GetAllTexts(langComp);
                }
            }

            if (GUILayout.Button("Reset Selection"))
            {
                langComp.TMPList.Clear();
                langComp.TextList.Clear();
                langComp.TextMeshList.Clear();
                langComp.TextsKeys.Clear();
            }
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();
            // TextMeshPro
            for (int i = 0; i < langComp.TMPList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Remove", GUILayout.Width(60))) { langComp.RemoveItem(langComp.TMPList[i]); } // Remove Button

                EditorGUI.BeginDisabledGroup(true);
                if (i < langComp.TMPList.Count) EditorGUILayout.ObjectField(langComp.TMPList[i], typeof(TextMeshProUGUI), true); // Game Object
                EditorGUI.EndDisabledGroup();

                if (KeyToText.ContainsKey(ref langComp.TextsKeys, "TMP" + i.ToString()))
                {
                    langComp.TextsKeys[KeyToText.GetStructIndex(ref langComp.TextsKeys, "TMP" + i.ToString())].Value = EditorGUILayout.TextField(KeyToText.GetValue(ref langComp.TextsKeys, "TMP" + i.ToString()));
                }


                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(5);

            // Text Legacy
            for (int i = 0; i < langComp.TextList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Remove", GUILayout.Width(60))) { langComp.RemoveItem(langComp.TextList[i]); } // Remove Button

                EditorGUI.BeginDisabledGroup(true);
                if (i < langComp.TextList.Count) EditorGUILayout.ObjectField(langComp.TextList[i], typeof(TextMeshProUGUI), true); // Game Object
                EditorGUI.EndDisabledGroup();

                if (KeyToText.ContainsKey(ref langComp.TextsKeys, "Text" + i.ToString()))
                {
                    langComp.TextsKeys[KeyToText.GetStructIndex(ref langComp.TextsKeys, "Text" + i.ToString())].Value = EditorGUILayout.TextField(KeyToText.GetValue(ref langComp.TextsKeys, "Text" + i.ToString())); // Key Value
                }

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(5);

            // TextMesh
            for (int i = 0; i < langComp.TextMeshList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Remove", GUILayout.Width(60))) { langComp.RemoveItem(langComp.TextMeshList[i]); } // Remove Button

                EditorGUI.BeginDisabledGroup(true);
                if (i < langComp.TextMeshList.Count) EditorGUILayout.ObjectField(langComp.TextMeshList[i], typeof(TextMeshProUGUI), true); // Game Object
                EditorGUI.EndDisabledGroup();

                if (KeyToText.ContainsKey(ref langComp.TextsKeys, "TextMesh" + i.ToString()))
                {
                    langComp.TextsKeys[KeyToText.GetStructIndex(ref langComp.TextsKeys, "TextMesh" + i.ToString())].Value = EditorGUILayout.TextField(KeyToText.GetValue(ref langComp.TextsKeys, "TextMesh" + i.ToString())); // Key Value
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }

    public class LocalizationWindow : EditorWindow
    {
        LocalizationComponent currentComp;

        List<TextMeshProUGUI> ChoiceTMPList = new List<TextMeshProUGUI>();
        List<Text> ChoiceTextList = new List<Text>();
        List<TextMesh> ChoiceTextMeshList = new List<TextMesh>();

        Vector2 scrollAll;
        Vector2 scrollTMP;
        Vector2 scrollText;
        Vector2 scrollTextMesh;

        bool[] CheckBoxs;

        public static LocalizationWindow ShowWindow()
        {
            LocalizationWindow window = GetWindow<LocalizationWindow>("Text GameObject List");
            window.minSize = new Vector2(214, 200);
            return window;
        }

        public void GetAllTexts(LocalizationComponent localComponent)
        {
            currentComp = localComponent;

            ChoiceTMPList = new List<TextMeshProUGUI>();
            ChoiceTextList = new List<Text>();
            ChoiceTextMeshList = new List<TextMesh>();

            TextMeshProUGUI[] TempoTMP = FindObjectsOfType<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI item in TempoTMP)
            {
                if (!currentComp.TMPList.Contains(item))
                {
                    ChoiceTMPList.Add(item);
                }
            }

            Text[] TempoText = FindObjectsOfType<Text>(true);
            foreach (Text item in TempoText)
            {
                if (!currentComp.TextList.Contains(item))
                {
                    ChoiceTextList.Add(item);
                }
            }

            TextMesh[] TempoTextMesh = FindObjectsOfType<TextMesh>(true);
            foreach (TextMesh item in TempoTextMesh)
            {
                if (!currentComp.TextMeshList.Contains(item))
                {
                    ChoiceTextMeshList.Add(item);
                }
            }

            CheckBoxs = new bool[ChoiceTMPList.Count + ChoiceTextList.Count + ChoiceTextMeshList.Count];
        }

        private bool GiveAllTexts()
        {
            if (currentComp == null) { return false; }

            int index = 0;

            foreach (TextMeshProUGUI TMP in ChoiceTMPList)
            {
                if (CheckBoxs[index])
                {
                    currentComp.TMPList.Add(TMP);
                    KeyToText.AddItem(ref currentComp.TextsKeys, "TMP" + (currentComp.TMPList.Count - 1).ToString(), "");
                }

                index++;
            }
            foreach (Text text in ChoiceTextList)
            {
                if (CheckBoxs[index])
                { 
                    currentComp.TextList.Add(text);
                    KeyToText.AddItem(ref currentComp.TextsKeys, "Text" + (currentComp.TextList.Count - 1).ToString(), "");
                }

                index++;
            }
            foreach (TextMesh textMesh in ChoiceTextMeshList)
            {
                if (CheckBoxs[index])
                {
                    currentComp.TextMeshList.Add(textMesh);
                    KeyToText.AddItem(ref currentComp.TextsKeys, "TextMesh" + (currentComp.TextMeshList.Count - 1).ToString(), "");
                }
                
                index++;
            }

            //string tmp = "ADD : ";
            //foreach (KeyToText item in currentComp.TextsKeys)
            //{
            //    tmp += item.Key + " | ";
            //}
            //Debug.Log(tmp);

            return true;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            scrollAll = EditorGUILayout.BeginScrollView(scrollAll);

            GUILayout.Label("Select all the desired GameObjects", EditorStyles.boldLabel);
            GUILayout.Space(10);

            int index = 0;

            //Show every TextMeshPro in scene
            if (ChoiceTMPList.Count > 0)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Label("[TextMeshPro]", EditorStyles.boldLabel);
                scrollTMP = EditorGUILayout.BeginScrollView(scrollTMP);

                for (int i = 0; i < ChoiceTMPList.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();

                    CheckBoxs[index] = EditorGUILayout.Toggle(CheckBoxs[index], GUILayout.Width(20));
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(ChoiceTMPList[i].gameObject, typeof(GameObject), true);
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndHorizontal();

                    index++;
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }
            //Show every Text legacy in scene
            if (ChoiceTextList.Count > 0) 
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Label("[Text Legacy]", EditorStyles.boldLabel);
                scrollText = EditorGUILayout.BeginScrollView(scrollText);

                for (int i = 0; i < ChoiceTextList.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();

                    CheckBoxs[index] = EditorGUILayout.Toggle(CheckBoxs[index], GUILayout.Width(20));
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(ChoiceTextList[i].gameObject, typeof(GameObject), true);
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndHorizontal();

                    index++;
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }
            //Show every TextMesh in scene
            if (ChoiceTextMeshList.Count > 0) 
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Label("[TextMesh]", EditorStyles.boldLabel);
                scrollTextMesh = EditorGUILayout.BeginScrollView(scrollTextMesh);

                for (int i = 0; i < ChoiceTextMeshList.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();

                    CheckBoxs[index] = EditorGUILayout.Toggle(CheckBoxs[index], GUILayout.Width(20));
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(ChoiceTextMeshList[i].gameObject, typeof(GameObject), true);
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndHorizontal();

                    index++;
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                GUILayout.Space(10);
            }

            if (ChoiceTMPList.Count + ChoiceTextList.Count + ChoiceTextMeshList.Count <= 0)
            {
                GUILayout.Label("No Text Assets detected or already selected in list");
            }


            // Bottom buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
            if (GUILayout.Button("Validate"))
            {
                if (GiveAllTexts())
                    Close();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
#endif
}