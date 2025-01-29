using GMSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinWave : MonoBehaviour
{
    [SerializeField] LineRenderer _line;
    [SerializeField] int _pointCount;
    [SerializeField] float _ValToObtein;
    [SerializeField] float _marge = 20;

    private float _amplitude = 1;
    private Vector3[] _points;

    void Start()
    {
        GameManager.Instance.GetSetWaveValidity = false;
    }

    private void Update()
    {
        Draw();
        //Debug.Log(GameManager.Instance.GetSetWheelValue);
        if(GameManager.Instance.GetSetWheelValue >= _ValToObtein - _marge && GameManager.Instance.GetSetWheelValue <= _ValToObtein + _marge)
        {
            if (GameManager.Instance.GetSetWaveValidity == false)
            {
                Debug.Log("Win");
                GameManager.Instance.GetSetWaveValidity = true;
            }
        }
        else if (GameManager.Instance.GetSetWaveValidity == true)
        {
            GameManager.Instance.GetSetWaveValidity = false;
        }
    }



    void Draw()
    {
        float xStart = 0;
        float tau = 2 * Mathf.PI;
        float xFinish = tau;

        _line.positionCount = _pointCount;
        for (int currentPoint = 0; currentPoint < _pointCount; currentPoint++)
        {
            float progress = currentPoint/ _pointCount-1;
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = _amplitude * Mathf.Sin(tau* GameManager.Instance.GetSetWheelValue * x);
            _line.SetPosition(currentPoint, new Vector3(x, y, 0));
        }
    }
}
