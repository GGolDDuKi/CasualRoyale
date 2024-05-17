using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Define;

public class GameManager
{
    public Authority Authority { get; set; } = Authority.Client;
    public bool InGame { get; set; } = false;
    public bool Ready { get; set; } = false;
    public int Rank { get; set; }
    public int CameraIndex { get; set; } = 0;

    public void SetGameUI()
    {
        GameObject go = GameObject.Find("OpenOption");
        if (go == null)
            Managers.UI.GenerateUI("OpenOption");

        go = GameObject.Find("ControlUI");
        if (go == null)
            Managers.UI.GenerateUI("ControlUI");

        go = GameObject.Find("OpenEmotions");
        if (go == null)
            Managers.UI.GenerateUI("OpenEmotions");
    }

    public void Init()
    {
        Managers.Game.InGame = false;
        Managers.Game.Ready = false;
        Managers.Game.Authority = Authority.Client;
    }
}
