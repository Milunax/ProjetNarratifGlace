using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private InputActionReference PrimaryTouch;
    [SerializeField] private InputActionReference TouchPosition;

    [SerializeField] private float _minimumDrag;

    private Vector2 _fingerPosition;
    private Vector2 _slideStartPos;
    private Vector2 _slideEndPos;

    private void OnEnable()
    {
        PrimaryTouch.action.started += OnFingerSlideStarted;
        //PrimaryTouch.action.performed += InputReceived;
        PrimaryTouch.action.canceled += OnFingerSlideEnded;

        //TouchPosition.action.started += OnFingerSlideStarted;
        TouchPosition.action.performed += UpdateFingerPosition;
        //TouchPosition.action.canceled += OnFingerSlideEnded;
    }

    private void OnDisable()
    {
        PrimaryTouch.action.started -= OnFingerSlideStarted;
        //PrimaryTouch.action.performed -= InputReceived;
        PrimaryTouch.action.canceled -= OnFingerSlideEnded;

        //TouchPosition.action.started -= OnFingerSlideStarted;
        TouchPosition.action.performed -= UpdateFingerPosition;
        //TouchPosition.action.canceled -= OnFingerSlideEnded;
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
        //Debug.Log(slideOffset);

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
