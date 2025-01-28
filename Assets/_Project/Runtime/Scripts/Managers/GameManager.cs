using UnityEngine;

namespace GameManagerSpace
{
    public class GameManager : MonoBehaviour
    {
        // Managers references //
        public static GameManager Instance;

        // Game Manager local variables //
        #region Signal
        private float _wheelValue;
        private Vector2 _wheelClamp;

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
        public bool _simonIsActive;
        public bool _simonIsSuccess;
        #endregion

        #region Labyrinthe
        public bool _labyrintheIsActive;
        public bool _labyrintheIsSuccess;
        #endregion

        #region Switchs
        public bool _switchsIsActive;
        public bool _switchsIsSuccess;
        #endregion

        #region Wires
        public bool _wiresIsActive;
        public bool _wiresIsSuccess;
        #endregion

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
        }


    }
}
