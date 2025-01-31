using UnityEngine;

[CreateAssetMenu(menuName = "InGame File Explorer/FE Text File")]
public class FileExplorerFileTextSO : FileExplorerDataSO
{
    [Header("Text File")]
    public string title;
    [TextArea(2, 20)] public string description;
    public Sprite image;
}
