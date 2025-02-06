using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class CircuitBreaker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Switchs> _switchs = new List<Switchs>();
    [SerializeField] private List<Image> _ledList = new List<Image>();
    [SerializeField] private Switchs _selectedSwitch;
    [SerializeField] private Image _selectedLED;

    [Header("Parameters")]
    [SerializeField] Color _selectedColor;
    [SerializeField] Color _defaultColor;

    public Switchs selectedSwitch { get {return _selectedSwitch;} private set {_selectedSwitch = value;} }
    public Image selectedLED { get {return _selectedLED;} private set {_selectedLED = value;} }

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
        int idPreviousSwitch = _switchs.IndexOf(selectedSwitch);
        int idNextSwitch = _switchs.IndexOf(selectedSwitch);
        int idPreviousLED = _ledList.IndexOf(selectedLED);
        int idNextLED = _ledList.IndexOf(selectedLED);

        switch (switchInfo)
        {
            case DIRECTIONAL_PAD_INFO.LEFT:
                int curentSwitch = idPreviousSwitch - 1;
                curentSwitch = curentSwitch >= _switchs.Count ? 0 : curentSwitch < 0 ? _switchs.Count - 1 : curentSwitch;
                selectedSwitch = _switchs[curentSwitch];
                //Debug.Log(idPreviousSwitch);

                int currentLED = idPreviousLED - 1;
                currentLED = currentLED >= _ledList.Count ? 0 : currentLED < 0 ? _ledList.Count - 1 : currentLED;
                selectedLED = _ledList[currentLED];
                //Debug.Log(idPreviousLED);
                if (selectedLED == _ledList[idPreviousLED])
                {
                    _ledList[idPreviousLED].DOColor(_selectedColor, .25f);
                }
                else if(selectedLED != _ledList[idPreviousLED])
                    _ledList[idPreviousLED].DOColor(_defaultColor, .25f);

                break;
            case DIRECTIONAL_PAD_INFO.RIGHT:
                idNextSwitch++;
                idNextSwitch = idNextSwitch >= _switchs.Count ? 0 : idNextSwitch < 0 ? _switchs.Count - 1 : idNextSwitch;
                selectedSwitch = _switchs[idNextSwitch];
                //Debug.Log(idNextSwitch);

                idNextLED++;
                idNextLED = idNextLED >= _ledList.Count ? 0 : idNextLED < 0 ? _ledList.Count - 1 : idNextLED;
                _selectedLED.DOColor(_selectedColor, .25f);
                selectedLED = _ledList[idNextLED];
                //Debug.Log(idNextLED);
                
                break;
        }
    }

}
