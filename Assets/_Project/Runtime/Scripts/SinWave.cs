using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinWave : MonoBehaviour
{
    [SerializeField] LineRenderer _line;
    [SerializeField] int _pointCount;
    [SerializeField] float _amplitude = 1;
    [SerializeField] float _frequency = 1;
    [SerializeField] Vector2 _xLimits = new Vector2(0, 1);

    private Vector3[] _points;
    float test;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        Draw();
    }

    void Draw()
    {
        float xStart = _xLimits.x;
        float tau = 2 * Mathf.PI;
        float xFinish = _xLimits.y;

        _line.positionCount = _pointCount;
        for (int currentPoint = 0; currentPoint < _pointCount; currentPoint++)
        {
            float progress = currentPoint/ _pointCount-1;
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = _amplitude * Mathf.Sin(tau*_frequency*x);
            _line.SetPosition(currentPoint, new Vector3(x, y, 0));
        }
    }
}
