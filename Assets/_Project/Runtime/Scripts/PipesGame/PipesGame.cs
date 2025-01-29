using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PipesGame : MonoBehaviour
{
    [SerializeField] private List<Pipe> _pipes = new List<Pipe>();
    [SerializeField] private Pipe _selectedPipe;

    public Pipe SelectedPipe { get { return _selectedPipe; } private set { _selectedPipe = value; /* A FAIRE UPDATE DE LA PIPE SELECTIONNEE */} }

    void Start()
    // Start is called before the first frame update
    {
        _pipes.AddRange(GetComponentsInChildren<Pipe>());
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
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
