

public class UICloseButton : Button
{
    public override void OnClick()
    {
        base.OnClick();
        transform.parent.gameObject.SetActive(false);
    }
}
