using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LocalizationPackage
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance;

        public enum startBehavior
        {
            Default,
            UserPC,
            HandChoosen
        }

        [Header("Localization Parameters")]
        [SerializeField] private SystemLanguage[] languages;
        [SerializeField] private SystemLanguage defaultLanguage;

        [Header("Localization Behaviors")]
        [SerializeField] private SystemLanguage currentLanguage;
        [SerializeField] private startBehavior startRoutine;
        [SerializeField] private LocalizationComponent LanguageSelection;

        [Header("Debug Parameters")]
        [SerializeField] bool refreshLogs = true;
        [SerializeField] bool returnDefaultLanguageIfPossible = false;
        [SerializeField] bool refreshAtStart = true;

        public delegate void OnRefreshDelegate(SystemLanguage language);
        public static event OnRefreshDelegate OnRefresh;

        private LocalizationComponent[] AllComponents;

        public SystemLanguage[] GetListOfLanguages { get => languages; }
        public string GetCurrentLanguage { get => currentLanguage.ToString(); }
        public string GetDefaultLanguage { get => defaultLanguage.ToString(); }
        public startBehavior SetStartRoutine { set => startRoutine = value; }
        public bool DebugGetReturnDefault { get => returnDefaultLanguageIfPossible; }

        private void OnValidate()
        {
            if (languages.Length <= 0)
            {
                Debug.LogError("Languages list is empty", this.gameObject);
                return;
            }

            if (defaultLanguage.ToString() == null || !languages.Contains<SystemLanguage>(defaultLanguage))
            {
                defaultLanguage = languages[0];
                Debug.LogWarning("Default language is not valid! \nSet to the first language in the list : " + defaultLanguage.ToString(), this.gameObject);
            }
        }

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);

            LocalizationManager.OnRefresh += RefreshCalled;

            SceneManager.sceneLoaded += ReStart;
        }
        private void OnDisable()
        {
            LocalizationManager.OnRefresh -= RefreshCalled;

            SceneManager.sceneLoaded -= ReStart;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                if (Instance != null) Destroy(gameObject);
                Instance = this;
            }
            else
            {
                Debug.LogError("Too many Localization manager instance ", gameObject);
            }
        }

        private void CheckLanguageRoutine()
        {
            switch (startRoutine)
            {
                case startBehavior.UserPC:
                    if (!languages.Contains<SystemLanguage>(Application.systemLanguage))
                    {
                        Debug.LogWarning("'" + Application.systemLanguage.ToString() + "' as system language is not supported, language set to default ", gameObject);
                        currentLanguage = defaultLanguage;
                        break;
                    }
                    currentLanguage = Application.systemLanguage;
                    break;

                case startBehavior.Default:
                    currentLanguage = defaultLanguage;
                    break;

                case startBehavior.HandChoosen:
                default:
                    if (!languages.Contains<SystemLanguage>(currentLanguage))
                    {
                        Debug.LogWarning("'" + currentLanguage + "' as choosen language is not supported, language set to default ", gameObject);
                        currentLanguage = defaultLanguage;
                        break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Change the start routine of the localization manager.
        /// </summary>
        /// <param name="routine">[Default = default language | UserPC = user's PC language or default if not supported | Hand Choosen = manually choosen or default if not supported]</param>
        /// <param name="callRefresh">Call refresh after changing start routine, true by default.</param>
        public void ChangeLanguageRoutine(startBehavior routine, bool callRefresh = true)
        {
            startRoutine = routine;
            CheckLanguageRoutine();
            if (callRefresh) CallRefresh();
        }

        private void Start()
        {
            AllComponents = FindObjectsOfType<LocalizationComponent>(true);
            StartCoroutine(WaitForInit());
        }
        private void ReStart(Scene scene, LoadSceneMode mode)
        {
            AllComponents = FindObjectsOfType<LocalizationComponent>(true);
            StartCoroutine(WaitForInit());
        }

        private IEnumerator WaitForInit()
        {
            bool condition = false;
            while (!condition)
            {
                for (int i = 0; i < AllComponents.Length; i++)
                {
                    condition = AllComponents[i].GetEndInit;

                    if (condition == false) break;
                }
                yield return new WaitForEndOfFrame();
            }

            CheckLanguageRoutine();
            if (refreshAtStart) CallRefresh();
            yield return null;
        }

        /// <summary>
        /// Give a text depending on the TSV file asked for, the key provided in parameter, and the current game language. TSV File must be in a LocalizationComponent present on the current scene.
        /// </summary>
        /// <param name="TSVFileName">Name of the TSV File wanted as a string. Name must not include the file extension.</param>
        /// <param name="Key">Given identification key corresponding to a text in the TSV file.</param>
        /// <returns>Text in current game language.</returns>
        public string UniGetText(string TSVFileName, string Key)
        {
            foreach(LocalizationComponent comp in AllComponents)
            {
                if (comp.GetTSVFileName == TSVFileName)
                {
                    return comp.GetTextSafe(Key);
                }
            }

            Debug.LogError("No TSV File named '" + TSVFileName + "'was find in '" + gameObject.name + "' ", gameObject);
            return "ERROR CAN'T FIND TSV: '" + TSVFileName + "' ";
        }

        /// <summary>
        /// Call the "Onrefresh" event.
        /// </summary>
        public void CallRefresh()
        {
            OnRefresh.Invoke(currentLanguage);
        }
        private void RefreshCalled(SystemLanguage language)
        {
            if (refreshLogs) Debug.Log("Refesh Localization : " + language);
        }

        /// <summary>
        ///  Change the language to the given parameter.
        /// </summary>
        /// <param name="newLanguage">New language desired.</param>
        /// <returns>[True : if language was changed | False : language is not supported & wasn't changed] </returns>
        public bool ChangeLanguage(SystemLanguage newLanguage)
        {
            if (!languages.Contains(newLanguage))
            {
                Debug.LogError(newLanguage.ToString() + " is not supported");
                return false;
            }

            currentLanguage = newLanguage;
            CallRefresh();

            return true;
        }

        /// <summary>
        /// Return the name of the asked language, usefull for language selection in game options.
        /// </summary>
        /// <param name="languageToGet">Language Desired.</param>
        /// <param name="sameAsSelectedLanguage">[False : returns the language name in current game language (Default) | True : returns name of language in native]</param>
        /// <returns>Name of desired language.</returns>
        public string GetLanguageForSelection(SystemLanguage languageToGet, bool sameAsSelectedLanguage = false /**Return text is the same language as the one selected */)
        {
            if (!languages.Contains(languageToGet))
            {
                Debug.LogWarning(languageToGet.ToString() + " is not supported, add this language in the variable 'languages' if you want it to be supported ", gameObject);
            }

            return LanguageSelection.GetTextUnsafe(languageToGet.ToString(), sameAsSelectedLanguage);
        }
    }
}
