using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Google.Protobuf.Protocol;

public class InputPassword : Button
{
    public string Password
    {
        get
        {
            TMP_InputField input = GetComponentInChildren<TMP_InputField>();
            return input.text;
        }
    }

    public override void OnClick()
    {
        base.OnClick();

        if (Managers.Room._selected == null)
            return;

        Room room = Managers.Room._selected;

        CS_EnterRoom enterPacket = new CS_EnterRoom();
        enterPacket.PlayerId = Managers.User.Id;
        enterPacket.RoomId = room.RoomId;
        enterPacket.PassWord = Password;

        Managers.Network.S_Send(enterPacket);
    }
}
