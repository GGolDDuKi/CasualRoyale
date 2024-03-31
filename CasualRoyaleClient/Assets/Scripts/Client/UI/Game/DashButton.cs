

public class DashButton : Button
{
    public override void OnClick()
    {
        MyPlayerController mp = Managers.Object.MyPlayer;

        StartCoroutine(mp.CoDash());
    }
}
