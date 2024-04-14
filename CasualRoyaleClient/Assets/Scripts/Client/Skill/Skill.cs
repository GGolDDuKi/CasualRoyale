using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public Sprite icon;
    public string skillName;
    public string description;
    public float coolTime;

    void Start()
    {
        Init();
    }

    public virtual void Init() { }

    public virtual void UsingSkill() { }
}
