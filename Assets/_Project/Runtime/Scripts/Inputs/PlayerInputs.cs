using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public InputActionReference primaryTouch;
    public InputActionReference touchPosition;
    public InputActionReference touchDelta;

    [SerializeField] private float _minimumDrag;

    private Vector2 _fingerPosition;
    private Vector2 _slideStartPos;
    private Vector2 _slideEndPos;
    /// <summary>
    /// Current acceleration vector of the cursor
    /// </summary>
    private Vector2 _fingerDelta;
    /// <summary>
    /// Difference between the start position & the current position
    /// </summary>
    private Vector2 _slideDeltaV;

    public Vector2 GetFingerPosition { get => _fingerPosition;}
    public Vector2 GetSlideStartPos { get => _slideStartPos;}
    public Vector2 GetSlideEndPos { get => _slideEndPos; }

    public Vector2 GetFingerDelta { get => _fingerDelta; }
    public Vector2 GetSlideDeltaV { get => _slideDeltaV;}

    private void OnEnable()
    {
        primaryTouch.action.started += OnFingerSlideStarted;
        primaryTouch.action.canceled += OnFingerSlideEnded;

        touchPosition.action.performed += UpdateFingerPosition;
        touchDelta.action.performed += UpdateFingerdelta;
    }

    private void OnDisable()
    {
        primaryTouch.action.started -= OnFingerSlideStarted;
        primaryTouch.action.canceled -= OnFingerSlideEnded;

        touchPosition.action.performed -= UpdateFingerPosition;
        touchDelta.action.performed -= UpdateFingerdelta;
    }

    private void UpdateFingerPosition(InputAction.CallbackContext ctx)
    {
        _fingerPosition = ctx.ReadValue<Vector2>();
        _slideDeltaV = _fingerPosition - _slideStartPos;
    }

    private void UpdateFingerdelta(InputAction.CallbackContext ctx)
    {
        _fingerDelta = ctx.ReadValue<Vector2>();
    }

    private void OnFingerSlideStarted(InputAction.CallbackContext ctx)
    {
        _slideStartPos = _fingerPosition;
        _slideDeltaV = Vector2.zero;
    }

    private void OnFingerSlideEnded(InputAction.CallbackContext ctx)
    {
        _slideEndPos = _fingerPosition;
        _slideDeltaV = _slideEndPos - _slideStartPos;

        _fingerDelta = DetectSlide();
    }

    //VALEURS DE RETOUR SUJETTES � CHANGEMENTS
    private Vector2 DetectSlide()
    {
        if (Mathf.Abs(_slideDeltaV.x) > Mathf.Abs(_slideDeltaV.y) && Mathf.Abs(_slideDeltaV.x) > _minimumDrag) //Si l'offset en x est plus grand qu'en y
        {
            if (_slideDeltaV.x > 0)
            {
                // Aller � droite
                Debug.Log("DROITE");
                return Vector2.right;
            }
            else if (_slideDeltaV.x < 0)
            {
                // Aller � gauche
                Debug.Log("GAUCHE");
                return Vector2.left;
            }
        }
        else if (Mathf.Abs(_slideDeltaV.x) < Mathf.Abs(_slideDeltaV.y) && Mathf.Abs(_slideDeltaV.y) > _minimumDrag) //Si l'offset en y est plus grand qu'en x
        {
            if (_slideDeltaV.y > 0)
            {
                // Aller en haut
                Debug.Log("HAUT");
                return Vector2.up;
            }
            else if (_slideDeltaV.y < 0)
            {
                // Aller en bas
                Debug.Log("BAS");
                return Vector2.down;
            }
        }
        // On bouge pas
        Debug.Log("BOUGE PAS");
        return Vector2.zero;
    }

    public GameObject Detection()
    {
        Ray ray = Camera.main.ScreenPointToRay(_fingerPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            return hit.transform.gameObject;
        }
        return null;
    }
}
