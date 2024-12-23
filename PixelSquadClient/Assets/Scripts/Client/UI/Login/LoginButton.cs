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
        C_Login loginPacket = new C_Login();
        loginPacket.Info = Managers.User.Info;
        loginPacket.Info.Name = inputName;
        Managers.Network.Send(loginPacket);
    }
}
