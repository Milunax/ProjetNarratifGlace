using System.Collections;
using System.Collections.Generic;
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

        [Header("Other Parameters")]
        [SerializeField] bool refreshLogs = true;

        public delegate void OnRefreshDelegate(SystemLanguage language);
        public static event OnRefreshDelegate OnRefresh;

        private LocalizationComponent[] AllComponents;

        public SystemLanguage[] GetListOfLanguages { get => languages; }
        public string GetCurrentLanguage { get => currentLanguage.ToString(); }
        public string GetDefaultLanguage { get => defaultLanguage.ToString(); }
        public startBehavior SetStartRoutine { set => startRoutine = value; }

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
        }
        private void OnDisable()
        {
            LocalizationManager.OnRefresh -= RefreshCalled;
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
                    break;
            }
        }

        private void Start()
        {
            AllComponents = FindObjectsOfType<LocalizationComponent>();
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

            CallRefresh();
            yield return null;
        }

        public void CallRefresh()
        {
            OnRefresh.Invoke(currentLanguage);
        }
        private void RefreshCalled(SystemLanguage language)
        {
            if (refreshLogs) Debug.Log("Refesh Localization : " + language);
        }
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

        public string GetLanguageForSelection(SystemLanguage languageToGet, bool sameAsSelectedLanguage = false /**Return text is the same language as the one selected */)
        {
            if (!languages.Contains(languageToGet))
            {
                Debug.LogWarning(languageToGet.ToString() + " is not supported, add this language in the variable 'languages' if you want it to be supported ", gameObject);
            }

            return LanguageSelection.GetText(languageToGet.ToString(), sameAsSelectedLanguage);
        }
    }
}
