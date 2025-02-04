using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    [SerializeField] PointOfInterest _up;
    [SerializeField] PointOfInterest _down;
    [SerializeField] PointOfInterest _right;
    [SerializeField] PointOfInterest _left;

    [SerializeField] private MAP_ACTIVE _task;

    public PointOfInterest Up => _up;
    public PointOfInterest Down => _down;
    public PointOfInterest Right => _right;
    public PointOfInterest Left => _left;
    public MAP_ACTIVE Task => _task;
}
