using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Skill
{
    public SkillInfo Info { get; private set; }
    public Sprite _icon;

    //���߿� Json���·� ��ų ������ �̾Ƽ� id�� Ű�� Dictionary���� ã�Ƽ� ���
    public virtual void Init(SkillInfo info)
    {
        Info = info;
        _icon = Managers.Resource.Load<Sprite>($"Sprites/SkillIcons/{Info.SkillName}");
    }

    public virtual void UsingSkill(int userId, Vector2 lastDir)
    {

    }
}
