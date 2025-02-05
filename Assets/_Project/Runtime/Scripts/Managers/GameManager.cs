using UnityEngine;
using NaughtyAttributes;

namespace GMSpace
{
    public class GameManager : MonoBehaviour
    {
        // Managers references //
        public static GameManager Instance;
        public static PlayerInputs playerInputs;
        public static SoundManager soundManager;

        // Game Manager local variables //
        #region GameState
        private GAME_STATE gameState;
        private PROGRESSION _progression = new PROGRESSION(0, 0, 0);

        public GAME_STATE GetGameState { get => gameState; }

        public PROGRESSION GetProgression { get => _progression; }
        public PROGRESSION SetByForceProgression { set => _progression = value; }
        public int GetSetProgressionDay 
        { 
            get => _progression.day;
            set
            {
                if (value > _progression.day)
                {
                    _progression.inDay = 0;
                    _progression.day = value;
                }
                else Debug.Log("Day wasn't changed : value is lower or equal to current day");
            }
        }
        public int SetByForceProgressionDay { set => _progression.day = value; }
        public int GetSetProgressionInDay 
        { 
            get => _progression.inDay;
            set
            {
                if (value > _progression.inDay)
                {
                    _progression.inDay = value;
                }
                else Debug.Log("inDay wasn't changed : value is lower or equal to current inDay");
            }
        }
        public int SetByForceProgressionInDay { set => _progression.inDay = value;  }
        #endregion
        #region NarrativeIA
        private bool _narrativeIAIsActive;

        //Accessor for ergonomy and possible debug purposes
        public bool GetSetNarrativeIAIsActive
        {
            get => _narrativeIAIsActive;
            set => _narrativeIAIsActive = value;
        }
        public int GetSetNarrativeIALevel
        {
            get => _progression.narrativeIALevel;
            set
            {
                if (value > _progression.narrativeIALevel)
                {
                    _progression.narrativeIALevel = value;
                }
                else Debug.Log("IA Level wasn't changed : value is lower or equal to current IA Level");
            }
        }
        public int SetByForceNarrativeIALevel { set => _progression.narrativeIALevel = value; }
        #endregion

        #region Switchs
        private bool _isSwitchActive = false;

        public bool switchshortCiruit 
        {
            get => _isSwitchActive;
            set => _isSwitchActive = value;
        }
        #endregion

        #region Signal
        [SerializeField] private float _wheelValue = 0f;
        private Vector2 _wheelClamp = new Vector3(0f, 100f);
        [SerializeField] private bool _wavesIsValid = false;

        public float GetSetWheelValue {
            get => _wheelValue;
            set => _wheelValue = Mathf.Clamp(value, _wheelClamp.x, _wheelClamp.y);
        }
        public float GetWheelMin { get => _wheelClamp.x; }
        public float GetWheelMax { get => _wheelClamp.y; }
        public bool SetWheelMinMax (float min, float max)
        {
            if (min >= max)
            {
                Debug.LogWarning("Given wheel min value is equal or superior from max value. ", gameObject);
                return false;
            }

            _wheelClamp.x = min;
            _wheelClamp.y = max;
            return true;
        }
        public bool GetSetWaveValidity 
        { 
            get => _wavesIsValid;
            set => _wavesIsValid = value;
        }
        #endregion
        #region Simon
        private bool _simonIsActive;
        private bool _simonIsSuccess;

        //Accessor for ergonomy and possible debug purposes
        public bool GetSetSimonIsActive
        {
            get => _simonIsActive;
            set => _simonIsActive = value;
        }
        public bool GetSetSimonIsSuccess
        {
            get => _simonIsSuccess;
            set => _simonIsSuccess = value;
        }
        #endregion
        #region Labyrinthe
        private bool _labyrintheIsActive;
        private bool _labyrintheIsSuccess;

        //Accessor for ergonomy and possible debug purposes
        public bool GetSetLabyrintheIsActive
        {
            get => _labyrintheIsActive;
            set => _labyrintheIsActive = value;
        }
        public bool GetSetLabyrintheIsSuccess
        {
            get => _labyrintheIsSuccess;
            set => _labyrintheIsSuccess = value;
        }
        #endregion
        #region Wires
        private bool _wiresIsActive;
        private bool _wiresIsSuccess;

        //Accessor for ergonomy and possible debug purposes
        public bool GetSetWiresIsActive
        {
            get => _wiresIsActive;
            set => _wiresIsActive = value;
        }
        public bool GetSetWiresIsSuccess
        {
            get => _wiresIsSuccess;
            set => _wiresIsSuccess = value;
        }
        #endregion

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
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
                Debug.LogError("Too many GameManager instance ", gameObject);
            }

            // Init Managers //
            if (TryGetComponent<PlayerInputs>(out PlayerInputs inputComp)) playerInputs = inputComp;
            else Debug.LogError("No PlayerInputs components was found in the GameManager", gameObject);

            if (TryGetComponent<SoundManager>(out SoundManager sound)) soundManager = sound;
            else Debug.LogError("No SoundManager components was found in the GameManager", gameObject);
        }

        private void Update()
        {
            //Debug.Log("GM WheelValue : " + _wheelValue);
        }
    }
}
