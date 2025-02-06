using UnityEngine;

public class ButtonsInteractions : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        Keypad.OnKeyPressed += KeyPadAnim;
        DirectionalPad.OnKeyPressed += DirectionalPadAnim;
        ContextualButtons.OnKeyPressed += ContextualAnim;
        SimonPad.OnKeyPressed += SimonAnim;
    }
    private void OnDisable()
    {
        Keypad.OnKeyPressed -= KeyPadAnim;
        DirectionalPad.OnKeyPressed -= DirectionalPadAnim;
        ContextualButtons.OnKeyPressed -= ContextualAnim;
        SimonPad.OnKeyPressed -= SimonAnim;
    }

    private void KeyPadAnim(char number)
    {
        animator.SetInteger("NumberKeypad", (int)number - 48);
        animator.SetTrigger("Keypad");
    }
    private void DirectionalPadAnim(DIRECTIONAL_PAD_INFO info)
    {
        animator.SetInteger("InfoDirectionalPad", (int)info);
        animator.SetTrigger("DirectionalPad");
    }
    private void ContextualAnim(CONTEXTUAL_INPUT_INFO info)
    {
        animator.SetInteger("InfoContextual", (int)info);
        animator.SetTrigger("Contextual");
    }
    private void SimonAnim(SIMON_PAD_INFO info)
    {
        animator.SetInteger("InfoSimon", (int)info);
        animator.SetTrigger("Simon");
    }
}
