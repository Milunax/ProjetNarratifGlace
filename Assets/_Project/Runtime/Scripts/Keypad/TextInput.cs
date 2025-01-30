
public class TextInput : BaseKeypadText
{
    protected override void Start()
    {
        base.Start();

        ResetText();
    }

    public void ResetText()
    {
        textComponent.text = "";
    }
}
