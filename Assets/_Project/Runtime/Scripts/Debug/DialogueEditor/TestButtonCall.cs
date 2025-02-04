using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButtonCall : MonoBehaviour
{
    [SerializeField] Button _button;

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            Debug.Log("Listener Called");
        });

        _button.onClick.Invoke();
    }



}
