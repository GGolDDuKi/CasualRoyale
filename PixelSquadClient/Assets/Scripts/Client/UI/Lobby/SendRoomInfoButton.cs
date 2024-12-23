using Google.Protobuf.Protocol;

public class SendRoomInfoButton : Button
{
    public override void OnClick()
    {
        base.OnClick();
        RoomInfo roomInfo = new RoomInfo();
        InputRoomInfo info = transform.parent.GetComponent<InputRoomInfo>();
        roomInfo = info.GetRoomInfo();
        C_MakeRoom roomPacket = new C_MakeRoom();
        roomPacket.Room = roomInfo;
        Managers.Network.Send(roomPacket);
    }
}
