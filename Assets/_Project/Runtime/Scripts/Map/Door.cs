using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool _isOpen;

    public bool IsOpen { get => _isOpen; set => _isOpen = value; }
}
