using UnityEngine;

public class FileExplorerDataSO : ScriptableObject
{
    [Header("Primary Values")]
    public string fileName;

    [Header("Accessibility")]
    public PROGRESSION isAccessible = new PROGRESSION(0, 0, 0);
    public PROGRESSION isVisible = new PROGRESSION(0, 0, 0);
    public bool manualLock;
    public bool manualIsUnlock;
}