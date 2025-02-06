using GMSpace;
using UnityEngine;

public class SimonButtons : MonoBehaviour
{
    [SerializeField] SIMON_PAD_INFO _inputSimon;

    public SIMON_PAD_INFO PressedInput()
    {
        SimonPatternBehaviour.SimonSound(_inputSimon);
        return _inputSimon;
    }
}
