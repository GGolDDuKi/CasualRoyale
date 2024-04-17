using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount
    }

    public enum UIEvent
    {
        Click,
        Drag
    }

    public class User
    {
        public UserInfo Info;
    }

    public class SkillInfo
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string SkillType { get; set; }
        public string Description { get; set; }
        public float Range { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float DmgRatio { get; set; }
        public float CoolTime { get; set; }
    }

    public class ClassInfo
    {
        public float MaxHp { get; set; }
        public float Speed { get; set; }
        public float Damage { get; set; }
        public int FirstSkillId { get; set; }
        public int SecondSkillId { get; set; }
    }
}
