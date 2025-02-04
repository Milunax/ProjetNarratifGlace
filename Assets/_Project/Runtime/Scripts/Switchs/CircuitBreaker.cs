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
using System.Linq;

public class CircuitBreaker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Switchs> _switchs = new List<Switchs>();
    [SerializeField] private List<Image> _ledList = new List<Image>();
    [SerializeField] private Switchs _selectedSwitch;
    [SerializeField] private Image _SelectedLED;

    [Header("Parameters")]
    [SerializeField] Color _SelectedColor;

    public Switchs selectedSwitch { get {return _selectedSwitch;} private set {_selectedSwitch = value;} }
    public Image selectedLED { get {return _SelectedLED;} private set {_SelectedLED = value;} }

    void OnEnable()
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
                Debug.Log("Left");
                int idPreviousSwitch = _switchs.IndexOf(selectedSwitch);
                int idPreviousLED = _ledList.IndexOf(selectedLED);
                if (idPreviousSwitch == 0)
                {
                    selectedSwitch = _switchs[_switchs.Count - 1];
                    selectedLED = _ledList[_ledList.Count - 1];
                }
                else
                {
                    selectedSwitch = _switchs[idPreviousSwitch - 1];
                    selectedLED = _ledList[idPreviousLED - 1];
                    Debug.Log(idPreviousSwitch);
                }

                break;
            case DIRECTIONAL_PAD_INFO.RIGHT:
                Debug.Log("Right");
                int idNextSwitch = _switchs.IndexOf(selectedSwitch);
                int idNextLED = _ledList.IndexOf(selectedLED);
                if(idNextSwitch == _switchs.Count - 1)
                {
                    selectedSwitch = _switchs[0];
                    selectedLED = _ledList[0];
                }
                else
                {
                    selectedSwitch = _switchs[idNextSwitch - 1];
                    selectedLED = _ledList[idNextLED - 1];
                    _SelectedLED.DOColor(_SelectedColor, .25f);
                    Debug.Log(idNextLED);
                }
                break;
        }
    }

}
