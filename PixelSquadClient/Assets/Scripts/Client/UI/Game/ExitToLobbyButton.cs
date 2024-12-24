using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class ExitToLobbyButton : Button
{
    public override void OnClick()
    {
        base.OnClick();

        ExitToLobby();
    }

    public void ExitToLobby()
    {
        C_LeaveGame leavePacket = new C_LeaveGame();
        leavePacket.Authority = Managers.Game.Authority;
        Managers.Network.Send(leavePacket);

        Managers.Object.Clear();
        Managers.Game.Init();
        Managers.Scene.LoadScene(Define.Scene.Lobby);
    }
}
