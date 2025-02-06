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
        if (_fingerDelta.magnitude <= 1) _fingerDelta = Vector2.zero;
    }

    private void OnFingerSlideStarted(InputAction.CallbackContext ctx)
    {
        //Debug.Log("on finger slide started");
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
                //Debug.Log("DROITE");
                return Vector2.right;
            }
            else if (_slideDeltaV.x < 0)
            {
                // Aller � gauche
                //Debug.Log("GAUCHE");
                return Vector2.left;
            }
        }
        else if (Mathf.Abs(_slideDeltaV.x) < Mathf.Abs(_slideDeltaV.y) && Mathf.Abs(_slideDeltaV.y) > _minimumDrag) //Si l'offset en y est plus grand qu'en x
        {
            if (_slideDeltaV.y > 0)
            {
                // Aller en haut
                //Debug.Log("HAUT");
                return Vector2.up;
            }
            else if (_slideDeltaV.y < 0)
            {
                // Aller en bas
                //Debug.Log("BAS");
                return Vector2.down;
            }
        }
        // On bouge pas
        //Debug.Log("BOUGE PAS");
        return Vector2.zero;
    }

    public GameObject Detection()
    {
        if (Camera.main.orthographic)
        {
            Vector3 screenPoint = new Vector3 (GetSlideStartPos.x, GetSlideStartPos.y, 0);
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
            if (Physics.Raycast(worldPoint, Camera.main.transform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                Debug.DrawRay(worldPoint, Camera.main.transform.forward * 10, Color.green, 10f);
                //Debug.Log(hit.transform.gameObject.name);
                return hit.transform.gameObject;
            }
            Debug.DrawRay(worldPoint, Camera.main.transform.forward * 1000, Color.red, 10f);
            return null;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(GetSlideStartPos.x, GetSlideStartPos.y, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.green, 10f);
                //Debug.Log(hit.transform.gameObject.name);
                return hit.transform.gameObject;
            }
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 10f);
            return null;
        }
    }
}
