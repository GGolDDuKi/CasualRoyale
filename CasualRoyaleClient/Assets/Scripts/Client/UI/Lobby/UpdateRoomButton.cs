

public class UpdateRoomButton : Button
{
    public override void OnClick()
    {
        Managers.Room.UpdateRoomList();
    }
}
