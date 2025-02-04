using DG.Tweening;
using GMSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public static event Action<GameObject> OnTaskFinished;

    [SerializeField] MAP_ACTIVE _mapState;
    [SerializeField] private Image _player;
    [SerializeField] private List<PointOfInterest> _points =  new List<PointOfInterest>();
    [SerializeField] private PointOfInterest _currentPoint;


    [Header("Tasks")]
    [SerializeField] private GameObject _taskSimon;
    [SerializeField] private GameObject _taskSwitch;
    [SerializeField] private GameObject _taskPipes;
    [SerializeField] private GameObject _taskNumberCode;
    [SerializeField] private GameObject _taskNumberLogic;
    [SerializeField] private GameObject _taskWaves;

    private void OnEnable()
    {
        _mapState = MAP_ACTIVE.MAP;
        DirectionalPad.OnKeyPressed += ReceiveInput;
        OnTaskFinished += TaskFinished;
    }

    private void OnDisable()
    {
        _mapState = MAP_ACTIVE.CLOSED;
        DirectionalPad.OnKeyPressed -= ReceiveInput;
    }

    // Start is called before the first frame update
    void Start()
    {
        _points.AddRange(FindObjectsOfType<PointOfInterest>());
        //UpdateSelectedPoint(_points[0]);
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

    public void ReceiveInput(DIRECTIONAL_PAD_INFO input)
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
            case DIRECTIONAL_PAD_INFO.CONFIRM:
                if (_currentPoint != null && _currentPoint.Task != MAP_ACTIVE.CLOSED) OpenTask(_currentPoint.Task);
                break;
            default:
                break;
        }
    }

    private void OpenTask(MAP_ACTIVE task)
    {
        switch (task)
        {
            case MAP_ACTIVE.DOOR_SIMON:
                if(_taskSimon != null) _taskSimon.SetActive(true);
                _mapState = MAP_ACTIVE.DOOR_SIMON;
                break;
            case MAP_ACTIVE.SWITCH:
                if (_taskSwitch != null) _taskSwitch.SetActive(true);
                _mapState = MAP_ACTIVE.SWITCH;
                break;
            case MAP_ACTIVE.PIPES:
                if (_taskPipes != null) _taskPipes.SetActive(true);
                _mapState = MAP_ACTIVE.PIPES;
                break;
            case MAP_ACTIVE.NUMBERS_CODE:
                if (_taskNumberCode != null) _taskNumberCode.SetActive(true);
                _mapState= MAP_ACTIVE.NUMBERS_CODE;
                break;
            case MAP_ACTIVE.NUMBERS_LOGIC:
                if (_taskNumberLogic != null) _taskNumberLogic.SetActive(true);
                _mapState = MAP_ACTIVE.NUMBERS_LOGIC;
                break;
            case MAP_ACTIVE.WAVES:
                if(_taskWaves != null) _taskWaves.SetActive(true);
                _mapState = MAP_ACTIVE.WAVES;
                break;
            default:
                break;
        }
    }

    public void TaskFinished(GameObject objectToHide)
    {
        _mapState = MAP_ACTIVE.MAP;
        objectToHide.SetActive(false);
    }
}
