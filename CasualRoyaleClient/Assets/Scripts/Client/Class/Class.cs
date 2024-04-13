using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Class : MonoBehaviour
{
    public abstract void FirstSkill();
    public abstract void SecondSkill();
    public abstract void ThirdSkill();
}

public class Skill
{
    public Sprite icon;
    public string name;
    public string description;
    public float coolTime;

}
