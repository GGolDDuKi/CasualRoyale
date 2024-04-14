using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPlayerButton : Button
{
    public override void OnClick()
    {
        base.OnClick();

        Managers.Game.CameraIndex++;
        if (Managers.Game.CameraIndex > Managers.Object.Players.Count - 1)
            Managers.Game.CameraIndex = 0;

        Camera.main.transform.SetParent(Managers.Object.Players[Managers.Game.CameraIndex].transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);
    }
}
