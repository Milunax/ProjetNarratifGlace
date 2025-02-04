using UnityEngine;

namespace GMSpace
{
    public class GameManager : MonoBehaviour
    {
        // Managers references //
        public static GameManager Instance;
        public static PlayerInputs playerInputs;

        // Game Manager local variables //
        #region NarrativeIA
        private bool _narrativeIAIsActive;
        private int _narrativeIALevel;

        //Accessor for ergonomy and possible debug purposes
        public bool GetSetNarrativeIAIsActive
        {
            get => _narrativeIAIsActive;
            set => _narrativeIAIsActive = value;
        }
        public int GetSetNarrativeIALevel
        {
            get => _narrativeIALevel;
            set => _narrativeIALevel = value;
        }
        #endregion

        #region GameState
        public enum GAME_STATE
        {
            WAKE_UP = 0,
            DISCUSS = 1,
            ROUTINE_TASK = 2,
            SECONDARY_TASK = 3
        }
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
            if (TryGetComponent<PlayerInputs>(out PlayerInputs inputComp))
            {
                playerInputs = inputComp;
            }
            else
            { 
                Debug.LogError("No PlayerInputs components was found in the GameManager", gameObject);
            }
        }

        private void Update()
        {
            //Debug.Log("GM WheelValue : " + _wheelValue);
        }
    }
}
