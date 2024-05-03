using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Screen.SetResolution(640, 360, false);
        Application.runInBackground = true;

        //GameObject host = GameObject.Find("Host");
        //if(host != null)
        //{
        //    host.transform.parent = Managers.Scene.CurrentScene.transform;
        //    host.transform.SetParent(null);
        //}
    }

    public override void Clear()
    {
        
    }
}
