using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InGame File Explorer/FE Folder")]
public class FileExplorerFolderSO : FileExplorerDataSO
{
    [Header("Folder")]
    public List<FileExplorerDataSO> childs;
}
