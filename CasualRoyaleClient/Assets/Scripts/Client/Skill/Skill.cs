using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public int _id;
    public Sprite _icon;
    public string _skillName;
    public string _description;
    public float _coolTime;

    //���߿� Json���·� ��ų ������ �̾Ƽ� id�� Ű�� Dictionary���� ã�Ƽ� ���
    public virtual void Init(int id, string name, string description, float coolTime)
    {
        _id = id;
        _skillName = name;
        _description = description;
        _coolTime = coolTime;
        _icon = Managers.Resource.Load<Sprite>($"Sprites/SkillIcons/{_skillName}");
    }

    public virtual void UsingSkill(int userId, Vector2 lastDir)
    {

    }
}
