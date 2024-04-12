using Google.Protobuf.Protocol;
using UnityEngine;
using TMPro;

public class InputRoomInfo : MonoBehaviour
{
    public TMP_InputField roomName;
    public TMP_InputField password;
    public TMP_InputField maxMember;

    public RoomInfo GetRoomInfo()
    {
        RoomInfo roomInfo = new RoomInfo();
        {
            //방 제목 설정
            if (roomName.text.Length == 0)
                roomInfo.RoomName = $"{Managers.User.Name}'s Room";
            else
                roomInfo.RoomName = roomName.text;

            //방 패스워드 설정
            if (password.text.Length == 0)
            {
                roomInfo.Password = "";
                roomInfo.SecretRoom = false;
            }
            else
            {
                roomInfo.Password = password.text;
                roomInfo.SecretRoom = true;
            }

            //방 인원 설정
            if (maxMember.text.Length == 0)
                roomInfo.MaxMember = 4;
            else
            {
                int num;
                if(int.TryParse(maxMember.text, out num))
                {
                    roomInfo.MaxMember = num;
                }
            }

            //호스트 정보 설정
            roomInfo.HostId = Managers.User.Id;
            roomInfo.HostName = Managers.User.Name;
            roomInfo.CurMember = 0;
        }
        return roomInfo;
    }
}
