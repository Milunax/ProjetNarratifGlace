using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class BaseKeypadText : MonoBehaviour
{
    protected TextMeshProUGUI _textComponent;
    [SerializeField] protected int _id;

    public TextMeshProUGUI textComponent { get { return _textComponent; } set { _textComponent = value; } }
    public int id => _id;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _textComponent = GetComponent<TextMeshProUGUI>();
        AddSelfToKeypadList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddSelfToKeypadList()
    {
        FindObjectOfType<Keypad>().AddTextInput(this);
    }
}
