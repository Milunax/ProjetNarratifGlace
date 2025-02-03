using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    [SerializeField] PointOfInterest _up;
    [SerializeField] PointOfInterest _down;
    [SerializeField] PointOfInterest _right;
    [SerializeField] PointOfInterest _left;

    [SerializeField] List<DIRECTIONAL_PAD_INFO> _possibleDirections;

    public PointOfInterest Up => _up;
    public PointOfInterest Down => _down;
    public PointOfInterest Right => _right;
    public PointOfInterest Left => _left;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
