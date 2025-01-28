using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private InputActionReference _primaryTouch;
    [SerializeField] private InputActionReference _touchPosition;

    [SerializeField] private float _minimumDrag;

    private Vector2 _fingerPosition;
    private Vector2 _slideStartPos;
    private Vector2 _slideEndPos;

    private void OnEnable()
    {
        _primaryTouch.action.started += OnFingerSlideStarted;
        _primaryTouch.action.canceled += OnFingerSlideEnded;

        _touchPosition.action.performed += UpdateFingerPosition;
    }

    private void OnDisable()
    {
        _primaryTouch.action.started -= OnFingerSlideStarted;
        _primaryTouch.action.canceled -= OnFingerSlideEnded;

        _touchPosition.action.performed -= UpdateFingerPosition;
    }

    private void UpdateFingerPosition(InputAction.CallbackContext ctx)
    {
        _fingerPosition = ctx.ReadValue<Vector2>();
    }

    private void OnFingerSlideStarted(InputAction.CallbackContext ctx)
    {
        _slideStartPos = _fingerPosition;
    }

    private void OnFingerSlideEnded(InputAction.CallbackContext ctx)
    {
        _slideEndPos = _fingerPosition;

        Vector2 dir = DetectSlide();
    }

    //VALEURS DE RETOUR SUJETTES À CHANGEMENTS
    private Vector2 DetectSlide()
    {
        Vector2 slideOffset = _slideEndPos - _slideStartPos;

        if (Mathf.Abs(slideOffset.x) > Mathf.Abs(slideOffset.y) && Mathf.Abs(slideOffset.x) > _minimumDrag) //Si l'offset en x est plus grand qu'en y
        {
            if (slideOffset.x > 0)
            {
                // Aller à droite
                Debug.Log("DROITE");
                return Vector2.right;
            }
            else if (slideOffset.x < 0)
            {
                // Aller à gauche
                Debug.Log("GAUCHE");
                return Vector2.left;
            }
        }
        else if (Mathf.Abs(slideOffset.x) < Mathf.Abs(slideOffset.y) && Mathf.Abs(slideOffset.y) > _minimumDrag) //Si l'offset en y est plus grand qu'en x
        {
            if (slideOffset.y > 0)
            {
                // Aller en haut
                Debug.Log("HAUT");
                return Vector2.up;
            }
            else if (slideOffset.y < 0)
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
}
