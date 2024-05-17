

using Google.Protobuf.Protocol;

public class EnterRoomButton : Button
{
    public override void OnClick()
    {
        base.OnClick();

        if (Managers.Room._selected == null)
            return;

        Room room = Managers.Room._selected;

        if(room.SecretRoom == true)
        {
            //TODO : �н����� �Է� ó��
            Managers.UI.GenerateUI("UI/InputPassword");
            return;
        }

        CS_EnterRoom enterPacket = new CS_EnterRoom();
        enterPacket.PlayerId = Managers.User.Id;
        enterPacket.RoomId = room.RoomId;

        Managers.Network.S_Send(enterPacket);
    }
}
