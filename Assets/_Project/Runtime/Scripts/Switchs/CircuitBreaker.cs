using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices.WindowsRuntime;
using GMSpace;
using UnityEditor.Experimental.GraphView;

public class CircuitBreaker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Switchs> _switchs = new List<Switchs>();
    [SerializeField] private Switchs _selectedSwitch;

    [Header("Parameters")]
    [SerializeField] private int _column;

    public Switchs selectedSwitch { get { return _selectedSwitch; } private set { _selectedSwitch = value; } }

    void Start()
    {
        DirectionalPad.OnKeyPressed += MoveBetweenSwitchs;
    }

    void OnDisable()
    {
        DirectionalPad.OnKeyPressed -= MoveBetweenSwitchs;
    }

    private void MoveBetweenSwitchs(DIRECTIONAL_PAD_INFO switchInfo)
    {
        switch (switchInfo)
        {
            case DIRECTIONAL_PAD_INFO.LEFT:
                //selectedSwitch;
                break;
        }
    }

}
