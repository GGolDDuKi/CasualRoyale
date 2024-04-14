using UnityEngine;
using TMPro;
using Google.Protobuf.Protocol;

public class LoginButton : Button
{
    public override void OnClick()
    {
        base.OnClick();
        GameObject nickname = GameObject.Find("Nickname");
        string inputName = nickname.GetComponent<TMP_InputField>().text;
        //TODO : 서버로 닉네임 전송 -> AcceptLogin패킷 받으면 로비씬 로딩, 방 목록 나열
        CS_Login loginPacket = new CS_Login();
        loginPacket.Info = Managers.User.Info;
        loginPacket.Info.Name = inputName;
        Managers.Network.S_Send(loginPacket);
    }
}
