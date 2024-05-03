using Google.Protobuf.Protocol;

public class SendRoomInfoButton : Button
{
    public override void OnClick()
    {
        base.OnClick();
        RoomInfo roomInfo = new RoomInfo();
        InputRoomInfo info = transform.parent.GetComponent<InputRoomInfo>();
        roomInfo = info.GetRoomInfo();
        CS_MakeRoom roomPacket = new CS_MakeRoom();
        roomPacket.Room = roomInfo;
        Managers.Network.S_Send(roomPacket);
    }
}
