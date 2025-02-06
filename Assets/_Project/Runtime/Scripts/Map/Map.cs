using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    //public static event Action<GameObject> OnTaskFinished;

    [SerializeField] MAP_ACTIVE _mapState;
    [SerializeField] private Image _player;
    [SerializeField] private List<PointOfInterest> _points =  new List<PointOfInterest>();
    [SerializeField] private PointOfInterest _currentPoint;


    [Header("Tasks")]
    [SerializeField] private SimonPatternBehaviour _taskSimon;
    [SerializeField] private GameObject _taskSwitch;
    [SerializeField] private GameObject _taskPipes;
    [SerializeField] private PasswordManager _taskNumberCode;
    [SerializeField] private GameObject _taskNumberLogic;
    [SerializeField] private GameObject _taskWaves;

    [Header("Hide on close")]
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _tasks;

    private void OnEnable()
    {
        _mapState = MAP_ACTIVE.MAP;
        //DirectionalPad.OnKeyPressed += ReceiveInput;
        //OnTaskFinished += TaskFinished;
        SimonPatternBehaviour.OnSimonEnd += TaskFinished;
        PasswordManager.OnPasswordEnd += TaskFinished;
    }

    private void OnDisable()
    {
        _mapState = MAP_ACTIVE.CLOSED;
        //DirectionalPad.OnKeyPressed -= ReceiveInput;
        //OnTaskFinished -= TaskFinished;
    }

    // Start is called before the first frame update
    void Start()
    {
        _points.AddRange(FindObjectsOfType<PointOfInterest>());
        //UpdateSelectedPoint(_points[0]);
    }

    public void Opening()
    {
        _background.SetActive(true);
        _tasks.SetActive(true);
        _mapState = MAP_ACTIVE.MAP;
    }

    public void Closing()
    {
        _background.SetActive(false);
        _tasks.SetActive(false);
        _mapState = MAP_ACTIVE.CLOSED;
    }

    private void UpdateSelectedPoint(PointOfInterest newPoint)
    {
        if(newPoint != null)
        {
            _currentPoint = newPoint;
            UpdatePlayerPosition();
        } 
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
                if (_currentPoint.DoorUp != null)
                {
                    Debug.Log("1");
                    if (_currentPoint.DoorUp.IsOpen)
                    {
                        Debug.Log("3");
                        UpdateSelectedPoint(_currentPoint.Up);
                    }
                    else
                    {
                        Debug.Log("Open Simon");
                        OpenTask(MAP_ACTIVE.DOOR_SIMON);
                    }
                }
                else
                {
                    Debug.Log("2");
                    UpdateSelectedPoint(_currentPoint.Up);
                }
                break;
            case DIRECTIONAL_PAD_INFO.DOWN:
                    if (_currentPoint.DoorDown != null)
                    {
                        Debug.Log("1");
                        if (_currentPoint.DoorDown.IsOpen)
                        {
                                Debug.Log("3");
                                UpdateSelectedPoint(_currentPoint.Down);
                        }
                        else
                        {
                            Debug.Log("Open Simon");
                            OpenTask(MAP_ACTIVE.DOOR_SIMON);
                        }
                    }
                    else
                    {
                        Debug.Log("2");
                        UpdateSelectedPoint(_currentPoint.Down);
                    }
                break;
            case DIRECTIONAL_PAD_INFO.RIGHT:
                if (_currentPoint.DoorRight != null)
                {
                    Debug.Log("1");
                    if (_currentPoint.DoorRight.IsOpen)
                    {
                        Debug.Log("3");
                        UpdateSelectedPoint(_currentPoint.Right);
                    }
                    else
                    {
                        Debug.Log("Open Simon");
                        OpenTask(MAP_ACTIVE.DOOR_SIMON);
                    }
                }
                else
                {
                    Debug.Log("2");
                    UpdateSelectedPoint(_currentPoint.Right);
                }
                break;
            case DIRECTIONAL_PAD_INFO.LEFT:
                if (_currentPoint.DoorLeft != null)
                {
                    Debug.Log("1");
                    if (_currentPoint.DoorLeft.IsOpen)
                    {
                        Debug.Log("3");
                        UpdateSelectedPoint(_currentPoint.Left);
                    }
                    else
                    {
                        Debug.Log("Open Simon");
                        OpenTask(MAP_ACTIVE.DOOR_SIMON);
                    }
                }
                else
                {
                    Debug.Log("2");
                    UpdateSelectedPoint(_currentPoint.Left);
                }
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
        if(!_currentPoint.IsTaskCompleted)
        {
            switch (task)
            {
                case MAP_ACTIVE.DOOR_SIMON:
                    if (_taskSimon != null) _taskSimon.Opening();
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
                    if (_taskNumberCode != null) _taskNumberCode.Opening();
                    _mapState = MAP_ACTIVE.NUMBERS_CODE;
                    break;
                case MAP_ACTIVE.NUMBERS_LOGIC:
                    if (_taskNumberLogic != null) _taskNumberLogic.SetActive(true);
                    _mapState = MAP_ACTIVE.NUMBERS_LOGIC;
                    break;
                case MAP_ACTIVE.WAVES:
                    if (_taskWaves != null) _taskWaves.SetActive(true);
                    _mapState = MAP_ACTIVE.WAVES;
                    break;
                default:
                    break;
            }
        }
    }

    public void TaskFinished(bool isFinished)
    {
        _mapState = MAP_ACTIVE.MAP;
        _currentPoint.IsTaskCompleted = true;
    }
}
