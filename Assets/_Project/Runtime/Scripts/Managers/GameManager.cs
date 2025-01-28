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

        #region Signal
        private float _wheelValue = 0f;
        private Vector2 _wheelClamp = new Vector3(0f, 100f);

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

        #region Switchs
        private bool _switchsIsActive;
        private bool _switchsIsSuccess;

        //Accessor for ergonomy and possible debug purposes
        public bool GetSetSwitchsIsActive
        {
            get => _switchsIsActive;
            set => _switchsIsActive = value;
        }
        public bool GetSetSwitchsIsSuccess
        {
            get => _switchsIsSuccess;
            set => _switchsIsSuccess = value;
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

    }
}
