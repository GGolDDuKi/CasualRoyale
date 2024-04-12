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
            //�� ���� ����
            if (roomName.text.Length == 0)
                roomInfo.RoomName = $"{Managers.User.Name}'s Room";
            else
                roomInfo.RoomName = roomName.text;

            //�� �н����� ����
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

            //�� �ο� ����
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

            //ȣ��Ʈ ���� ����
            roomInfo.HostId = Managers.User.Id;
            roomInfo.HostName = Managers.User.Name;
            roomInfo.CurMember = 0;
        }
        return roomInfo;
    }
}
