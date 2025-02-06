using GMSpace;
using UnityEngine;

public class DirectionalPadButton : MonoBehaviour
{
    [SerializeField] DIRECTIONAL_PAD_INFO _inputDirection;

    public DIRECTIONAL_PAD_INFO PressedInput()
    {
        GameManager.soundManager.PlayAudio("click sound", SoundManager.AUDIO_CATEGORY.NONE, 0.05f);
        return _inputDirection;
    }
}
