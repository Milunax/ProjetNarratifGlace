using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PipesGame : MonoBehaviour
{
    [SerializeField] private List<Pipe> _pipes = new List<Pipe>();
    [SerializeField] private Pipe _selectedPipe;

    [SerializeField] private GameObject _pipesContainer;
    [SerializeField] private GameObject _pipePrefab;
    [SerializeField] private float _spawnOffset;

    [SerializeField] private int _gridRow;
    [SerializeField] private int _gridColumn;
    [SerializeField] private List<Pipe[]> _pipesGrid = new List<Pipe[]>();

    public Pipe SelectedPipe { get { return _selectedPipe; } private set { _selectedPipe = value; /* A FAIRE UPDATE DE LA PIPE SELECTIONNEE */} }

    void Start()
    // Start is called before the first frame update
    {
        FindObjectOfType<DirectionalPad>().OnKeyPressed += MoveBetweenPipes;

        for(int i = 0; i < _gridRow; i++)
        {
            Pipe[] newRow = new Pipe[_gridColumn];
            _pipesGrid.Add(newRow);

            for(int j = 0; j < _gridColumn; j++)
            {
                GameObject createdObject = Instantiate(_pipePrefab, new Vector3(transform.position.x + _spawnOffset * j, transform.position.y + _spawnOffset * i, transform.position.z),Quaternion.identity, _pipesContainer.transform);
                _pipesGrid[i][j] = createdObject.GetComponent<Pipe>(); 
            }
        }

        /*_pipes.AddRange(GetComponentsInChildren<Pipe>());
        _pipes = _pipes.OrderBy(e => e.Id).ToList();

        for(int i = 0; i < _pipes.Count; i++)
        {
            //Si on est sur la première (départ) pipe on assigne uniquement la pipe suivante
            if (_pipes[i].Id == 0)
            {
                _pipes[i].NextPipe = _pipes[i+1];
                continue;
            }

            //si on est sur la dernière (arrivée) pipe on assigne uniquement la précédente
            if (_pipes[i].Id == _pipes.Count - 1)
            {
                _pipes[i].PreviousPipe = _pipes[i-1];
                continue;
            }

            _pipes[i].NextPipe = _pipes[i+1];
            _pipes[i].PreviousPipe = _pipes[i-1];
        }*/
    }
    

    private void MoveBetweenPipes(DIRECTIONAL_PAD_INFO directionInfo)
    {

    }
}
