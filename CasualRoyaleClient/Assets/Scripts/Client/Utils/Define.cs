using Google.Protobuf.Protocol;
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

    public class SkillData : ILoader<int, SkillInfo>
    {
        public List<SkillInfo> skills = new List<SkillInfo>();

        public Dictionary<int, SkillInfo> MakeDictionary()
        {
            Dictionary<int, SkillInfo> dict = new Dictionary<int, SkillInfo> ();
            
            foreach (SkillInfo skill in skills)
                dict.Add(skill.SkillId, skill);

            return dict;
        }
    }

    public class ClassData : ILoader<string, ClassInfo>
    {
        public List<ClassInfo> classes = new List<ClassInfo>();

        public Dictionary<string, ClassInfo> MakeDictionary()
        {
            Dictionary<string, ClassInfo> dict = new Dictionary<string, ClassInfo>();

            foreach (ClassInfo job in classes)
                dict.Add(job.Class, job);

            return dict;
        }
    }

    public class SkillInfo
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string Class { get; set; }
        public string Description { get; set; }
        public float CoolTime { get; set; }
    }

    public class ClassInfo
    {
        public string Class { get; set; }
        public float MaxHp { get; set; }
        public float Speed { get; set; }
        public float Damage { get; set; }
        public int[] SkillIds = new int[3];
    }
}
