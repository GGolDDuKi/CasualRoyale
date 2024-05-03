using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : Button
{
    public override void OnClick()
    {
        if(Managers.Object.MyPlayer.Attack())
            StartCoroutine(transform.GetChild(1).GetComponent<SkillCooldown>().CoCooldown(Managers.Data.ClassData[Managers.Object.MyPlayer.Class.ToString()].AttackDelay));
    }
}
