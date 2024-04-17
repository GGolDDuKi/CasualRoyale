using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Skill
{
    public SkillInfo Info { get; private set; }
    public Sprite _icon;

    //나중에 Json형태로 스킬 데이터 뽑아서 id를 키로 Dictionary에서 찾아서 사용
    public virtual void Init(SkillInfo info)
    {
        Info = info;
        _icon = Managers.Resource.Load<Sprite>($"Sprites/SkillIcons/{Info.SkillName}");
    }

    public virtual void UsingSkill(int userId, Vector2 lastDir)
    {

    }
}
