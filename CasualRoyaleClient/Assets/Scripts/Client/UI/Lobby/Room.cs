using Google.Protobuf.Protocol;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Room : Button
{
    public int RoomId { get; set; }
    public string Name { get; set; }
    public string HostName { get; set; }
    public string Password { get; set; }
    public bool SecretRoom { get; set; }
    public int CurMember { get; set; }
    public int MaxMember { get; set; }

    public void Init(RoomInfo info)
    {
        RoomId = info.RoomId;
        Name = info.RoomName;
        HostName = info.HostName;
        Password = info.Password;
        SecretRoom = info.SecretRoom;
        CurMember = info.CurMember;
        MaxMember = info.MaxMember;

        UpdateRoomUI();
    }

    public override void OnClick()
    {
        if (Managers.Room._selected != GetComponent<Room>())
        {
            Managers.Room._selected = GetComponent<Room>();
        }
        else
            Managers.Room._selected = null;
    }

    public void UpdateRoomUI()
    {
        TextMeshProUGUI roomName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI hostName = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Image secretRoom = transform.GetChild(2).GetComponent<Image>();
        TextMeshProUGUI member = transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        roomName.text = Name;
        hostName.text = HostName;
        if(SecretRoom)//비밀방이면 잠금 표시
        {
            Color color = secretRoom.color;
            color.a = 1f;
            secretRoom.color = color;
        }
        else
        {
            Color color = secretRoom.color;
            color.a = 0f;
            secretRoom.color = color;
        }
        member.text = $"{CurMember}/{MaxMember}";
    }
}
