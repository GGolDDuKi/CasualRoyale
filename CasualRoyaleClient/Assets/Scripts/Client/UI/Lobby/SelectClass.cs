using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Google.Protobuf.Protocol;

public class SelectClass : MonoBehaviour
{
    string job;

    private void Start()
    {
        GetComponent<TMP_Dropdown>().options.Clear();

        List<string> classList = new List<string>(Managers.Data.ClassData.Keys);
        GetComponent<TMP_Dropdown>().AddOptions(classList);
        Managers.User.Job = JobType.Knight;
    }

    public void OnChangeClass()
    {
        job = GetComponent<TMP_Dropdown>().options[GetComponent<TMP_Dropdown>().value].text;
        Managers.User.Job = (JobType)Enum.Parse(typeof(JobType), job);
    }
}
