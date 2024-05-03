using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class ExitToLobbyButton : Button
{
    public override async void OnClick()
    {
        base.OnClick();

        StartCoroutine(ExitToLobby());
    }

    public IEnumerator ExitToLobby()
    {
        GameObject host = GameObject.Find("Host");
        if (host != null)
        {
            HC_MissingHost missingHost = new HC_MissingHost();
            missingHost.HostId = Managers.Object.MyPlayer.Id;
            host.GetComponent<HostPlayer>().room.Broadcast(missingHost);
            HS_EndGame endPacket = new HS_EndGame();
            endPacket.Room = Managers.Room.MyRoom;
            Managers.Network.S_Send(endPacket);
            yield return new WaitUntil(() => host.GetComponent<HostPlayer>().Clear());
        }
        else
        {
            CH_ExitRoom exitPacket = new CH_ExitRoom();
            exitPacket.Player = Managers.User.Info;
            Managers.Network.H_Send(exitPacket);
        }

        Managers.Object.Clear();
        CS_EndGame packet = new CS_EndGame();
        Managers.Network.S_Send(packet);
        Managers.Game.InGame = false;
        Managers.Scene.LoadScene(Define.Scene.Lobby);
    }
}
