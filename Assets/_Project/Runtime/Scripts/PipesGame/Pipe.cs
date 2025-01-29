using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private bool _isSelectable;

    [SerializeField] private Pipe _previousPipe;
    [SerializeField] private Pipe _nextPipe;

    #region PROPERTIES
    public int Id { get { return _id; } }

    public Pipe PreviousPipe { get { return _previousPipe; } set { _previousPipe = value; } }
    public Pipe NextPipe { get { return _nextPipe; } set { _nextPipe = value; } }
    #endregion

    //CONTINUER A PARTIR DE L'UPDATE A LA SELECTION
    public void UpdatePipe()
    {

    }
}
