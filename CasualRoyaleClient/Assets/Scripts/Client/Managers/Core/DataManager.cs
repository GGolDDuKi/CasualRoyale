using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Define;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDictionary();
}

public class DataManager
{
    public Dictionary<int, SkillInfo> SkillData { get; private set; } = new Dictionary<int, SkillInfo>();
    public Dictionary<int, Skill> Skills { get; private set; } = new Dictionary<int, Skill>();

    public Dictionary<string, ClassInfo> ClassData { get; private set; } = new Dictionary<string, ClassInfo>();

    public void Init()
    {
        SkillData = LoadJson<SkillData, int, SkillInfo>("Data/SkillData").MakeDictionary();
        //TODO : SkillInfo 정보따라 리플렉션 사용해서 스킬 인스턴스 생성 후 저장

        ClassData = LoadJson<ClassData, string, ClassInfo>("Data/ClassData").MakeDictionary();
        //TODO : 위와 동일
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = null;

        if (path.Contains("Data/"))
            textAsset = Managers.Resource.Load<TextAsset>(path);
        else
            textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");

        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
