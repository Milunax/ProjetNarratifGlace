using DG.Tweening;
using GMSpace;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [SerializeField] private Image _player;
    [SerializeField] private List<PointOfInterest> _points =  new List<PointOfInterest>();
    [SerializeField] private PointOfInterest _currentPoint;


    private void OnEnable()
    {
        DirectionalPad.OnKeyPressed += ReceiveInput;
    }

    private void OnDisable()
    {
        DirectionalPad.OnKeyPressed -= ReceiveInput;
    }

    // Start is called before the first frame update
    void Start()
    {
        _points.AddRange(FindObjectsOfType<PointOfInterest>());
        UpdateSelectedPoint(_points[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateSelectedPoint(PointOfInterest newPoint)
    {
        _currentPoint = newPoint;
        UpdatePlayerPosition();
    }

    private void UpdatePlayerPosition()
    {
        _player.rectTransform.position = _currentPoint.transform.position;
    }

    private void ReceiveInput(DIRECTIONAL_PAD_INFO input)
    {
        switch(input)
        {
            case DIRECTIONAL_PAD_INFO.UP:
                if(_currentPoint.Up != null) UpdateSelectedPoint(_currentPoint.Up);
                break;
            case DIRECTIONAL_PAD_INFO.DOWN:
                if (_currentPoint.Down != null) UpdateSelectedPoint(_currentPoint.Down);
                break;
            case DIRECTIONAL_PAD_INFO.RIGHT:
                if (_currentPoint.Right != null) UpdateSelectedPoint(_currentPoint.Right);
                break;
            case DIRECTIONAL_PAD_INFO.LEFT:
                if (_currentPoint.Left != null) UpdateSelectedPoint(_currentPoint.Left);
                break;
            default:
                break;
        }
    }
}
