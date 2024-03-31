using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Screen.SetResolution(640, 360, false);
        Application.runInBackground = true;
    }

    public override void Clear()
    {
        
    }
}
