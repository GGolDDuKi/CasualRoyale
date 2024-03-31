

public class UIDestroyButton : Button
{
    public override void OnClick()
    {
        Managers.Resource.Destroy(transform.parent.gameObject);
    }
}
