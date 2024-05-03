using UnityEngine;

public class LobbyScene : BaseScene
{
    private void Start()
    {
        Managers.Room.UpdateRoomList();
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Lobby;

        Screen.SetResolution(640, 360, false);
        Application.runInBackground = true;
    }

    public override void Clear()
    {

    }
}
