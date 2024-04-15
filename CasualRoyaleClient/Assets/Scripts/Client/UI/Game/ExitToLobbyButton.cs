using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitToLobbyButton : Button
{
    public override void OnClick()
    {
        base.OnClick();

        GameObject host = GameObject.Find("Host");
        if (host != null)
        {
            HC_MissingHost missingHost = new HC_MissingHost();
            missingHost.HostId = Managers.Object.MyPlayer.Id;
            host.GetComponent<HostPlayer>().room.Broadcast(missingHost);
            host.GetComponent<HostPlayer>().Clear();
        }

        Managers.Object.Clear();
        Managers.Network.Clear();
        //Managers.Game.Host = false;

        Managers.Scene.LoadScene(Define.Scene.Lobby);
    }
}
