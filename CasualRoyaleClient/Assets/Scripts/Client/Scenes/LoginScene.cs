using UnityEngine;

public class LoginScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;

        Screen.SetResolution(640, 360, false);
        Application.runInBackground = true;
    }

    public override void Clear()
    {

    }
}
