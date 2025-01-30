using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private bool _isSelectable;
    [SerializeField] private bool _isFilled;

    [SerializeField] private Pipe _previousPipe;
    [SerializeField] private Pipe _nextPipe;

    private int[] _possibleStartRotations = { 0, 90, 180, 270};

    #region PROPERTIES
    public int Id { get { return _id; } }

    public Pipe PreviousPipe { get { return _previousPipe; } set { _previousPipe = value; } }
    public Pipe NextPipe { get { return _nextPipe; } set { _nextPipe = value; } }
    #endregion

    private void Start()
    {
        transform.eulerAngles = new Vector3 (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, _possibleStartRotations[Random.Range(0, 4)]);
    }

    //CONTINUER A PARTIR DE L'UPDATE A LA SELECTION
    public void UpdatePipe()
    {

    }

    public void RotatePipe()
    {
        transform.eulerAngles += new Vector3(0, 0, 90);
    }
}
