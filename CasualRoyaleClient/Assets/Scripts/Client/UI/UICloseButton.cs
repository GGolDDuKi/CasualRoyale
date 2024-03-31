

public class UICloseButton : Button
{
    public override void OnClick()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
