using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : Button
{
    public override void OnClick()
    {
        if (Managers.Object.MyPlayer.State == Google.Protobuf.Protocol.ActionState.Attack)
            return;

        Managers.Object.MyPlayer.Attack();
    }
}
