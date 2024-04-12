using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorButton : Button
{
    public override void OnClick()
    {
        Managers.UI.GenerateUI("UI/MonitorUI");
        Destroy(GameObject.Find("GameEnd"));

        Managers.Game.CameraIndex = 0;
        Camera.main.transform.SetParent(Managers.Object.Players[Managers.Game.CameraIndex].transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);
    }
}
