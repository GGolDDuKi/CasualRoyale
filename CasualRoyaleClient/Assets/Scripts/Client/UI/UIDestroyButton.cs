

public class UIDestroyButton : Button
{
    public override void OnClick()
    {
        base.OnClick();
        Managers.Resource.Destroy(transform.parent.gameObject);
    }
}
