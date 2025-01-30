using TMPro;
using UnityEngine;

[System.Serializable]
public class BaseKeypadText : MonoBehaviour
{
    protected TextMeshProUGUI _textComponent;
    [SerializeField] protected int _id;

    public TextMeshProUGUI textComponent { get { return _textComponent; } set { _textComponent = value; } }
    public int id => _id;

    protected virtual void Start()
    {
        _textComponent = GetComponent<TextMeshProUGUI>();
        AddSelfToKeypadList();
    }

    private void AddSelfToKeypadList()
    {
        FindObjectOfType<PasswordManager>().AddTextToList(this);
    }
}
