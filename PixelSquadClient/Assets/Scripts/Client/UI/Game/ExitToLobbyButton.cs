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

        StartCoroutine(ExitToLobby());
    }

    public IEnumerator ExitToLobby()
    {
        GameObject host = GameObject.Find("Host");
        if (host != null)
        {
            //방장 퇴장 관리
            //yield return new WaitUntil(() => host.GetComponent<HostPlayer>().Clear());
            yield return null;
            HS_EndGame endPacket = new HS_EndGame();
            endPacket.Room = Managers.Room.MyRoom;
            Managers.Network.Send(endPacket);
        }
        else
        {
            CH_ExitRoom exitPacket = new CH_ExitRoom();
            exitPacket.Player = Managers.User.Info;
            Managers.Network.Send(exitPacket);
        }

        Managers.Object.Clear();
        CS_LeaveGame packet = new CS_LeaveGame();
        Managers.Network.Send(packet);
        Managers.Game.Init();
        Managers.Scene.LoadScene(Define.Scene.Lobby);
    }
}
