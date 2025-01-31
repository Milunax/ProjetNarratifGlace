using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] private int _row;
    [SerializeField] private int _col;

    [SerializeField] private bool _isSelectable = true;
    [SerializeField] private bool _isFilled;

    [SerializeField] private Pipe _previousPipe;
    [SerializeField] private Pipe _nextPipe;

    private int[] _possibleStartRotations = { 0, 90, 180, 270};

    #region PROPERTIES

    public int Row { get => _row; set => _row = value; }
    public int Col { get { return _col; } set { _col = value; } }
    public Pipe PreviousPipe { get { return _previousPipe; } set { _previousPipe = value; } }
    public Pipe NextPipe { get { return _nextPipe; } set { _nextPipe = value; } }
    public bool IsSelectable { get => _isSelectable; set => _isSelectable = value; }
    
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
