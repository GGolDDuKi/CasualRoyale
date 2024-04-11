using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitToLobbyButton : Button
{
    public override void OnClick()
    {
        Managers.Object.Clear();
        Managers.Scene.LoadScene(Define.Scene.Lobby);
    }
}
