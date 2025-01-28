using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WheelBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] EventSystem _eventSystem;

    private bool _isWeel = false;
    private Vector2 _mousePos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mousePos = Input.mousePosition;
            Detection();
        }
        else if (Input.GetMouseButtonUp(0))
        {

        }
    }

    void Detection()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        if(Physics.Raycast(ray, out RaycastHit hit , Mathf.Infinity))
        {
            Debug.Log(hit.transform.position);
            if(hit.collider == this)
            {
                Debug.Log("test");
            }
        }
    }
}
