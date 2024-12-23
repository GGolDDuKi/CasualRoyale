using Google.Protobuf.Protocol;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Application.runInBackground = true;
        Managers.Game.SetGameUI();

        C_SendInfo infoPacket = new C_SendInfo();
        infoPacket.Job = Managers.User.Job;
        infoPacket.Name = Managers.User.Name;
        Managers.Network.Send(infoPacket);
    }

    public override void Clear()
    {
        
    }
}
