using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstSkillButton : Button
{
    public void Init()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>($"Sprites/SkillIcons/{Managers.Data.SkillData[Managers.Object.MyPlayer.FirstSkillId].SkillName}");
    }

    public override void OnClick()
    {
        if(Managers.Object.MyPlayer.UsingSkill(Managers.Object.MyPlayer.FirstSkillId))
            StartCoroutine(transform.GetChild(1).GetComponent<SkillCooldown>().CoCooldown(Managers.Data.SkillData[Managers.Object.MyPlayer.FirstSkillId].CoolTime));
    }
}
