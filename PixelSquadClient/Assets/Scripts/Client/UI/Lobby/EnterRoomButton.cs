

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
            //TODO : 패스워드 입력 처리
            Managers.UI.GenerateUI("UI/InputPassword");
            return;
        }

        C_EnterRoom enterPacket = new C_EnterRoom();
        enterPacket.PlayerId = Managers.User.Id;
        enterPacket.RoomId = room.RoomId;

        Managers.Game.InGame = true;

        Managers.Network.Send(enterPacket);
    }
}
