using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Newtonsoft.Json;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDictionary();
}

public class DataManager
{
    public Dictionary<int, SkillInfo> SkillData = new Dictionary<int, SkillInfo>();
    public Dictionary<string, ClassInfo> ClassData = new Dictionary<string, ClassInfo>();

    public void Init()
    {
        //스킬 데이터 초기화
        SkillData = LoadJson<int, SkillInfo>("Data/SkillData");

        //직업 데이터 초기화
        ClassData = LoadJson<string, ClassInfo>("Data/ClassData");
        //필요할 경우, 나중에 게임 씬에서만 해당 정보들이 활용되도록 씬 전환마다 클리어 해주는 것도 고려 (메모리때문에)
    }

    Dictionary<Key, Value> LoadJson<Key, Value>(string path)
    {
        TextAsset jsonFile = Managers.Resource.Load<TextAsset>(path);
        Dictionary<Key, Value> data = new Dictionary<Key, Value>();
        data = JsonConvert.DeserializeObject<Dictionary<Key, Value>>(jsonFile.text);
        return data;
    }
}
