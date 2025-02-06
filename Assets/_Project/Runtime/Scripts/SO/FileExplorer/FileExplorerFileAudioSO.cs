using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InGame File Explorer/FE Audio File")]
public class FileExplorerAudioFileSO : FileExplorerDataSO
{
    [Header("Audio File")]
    public string title;
    public AudioClip audio;

    public List<AUDIO_TRANSCRIPTION> transcriptions;
}
