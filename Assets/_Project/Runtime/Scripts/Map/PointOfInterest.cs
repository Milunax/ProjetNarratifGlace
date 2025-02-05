using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointOfInterest : MonoBehaviour
{
    [SerializeField] PointOfInterest _up;
    [SerializeField] PointOfInterest _down;
    [SerializeField] PointOfInterest _right;
    [SerializeField] PointOfInterest _left;

    [SerializeField] Door _doorUp;
    [SerializeField] Door _doorDown;
    [SerializeField] Door _doorRight;
    [SerializeField] Door _doorLeft;

    [SerializeField] bool _isTaskCompleted;

    [SerializeField] private MAP_ACTIVE _task;

    public PointOfInterest Up => _up;
    public PointOfInterest Down => _down;
    public PointOfInterest Right => _right;
    public PointOfInterest Left => _left;
    public Door DoorUp { get => _doorUp; set => _doorUp = value; }
    public Door DoorDown { get => _doorDown; set => _doorDown = value; }
    public Door DoorRight { get => _doorRight; set => _doorRight = value; }
    public Door DoorLeft { get => _doorLeft; set => _doorLeft = value; }
    public MAP_ACTIVE Task => _task;
    public bool IsTaskCompleted { get => _isTaskCompleted; set => _isTaskCompleted = value; }


    public void UpdateDoor(bool doorState, Door door)
    {
        if(doorState && door.TryGetComponent(out Image image))
        {
            image.color = Color.green;
        }
    }
}
