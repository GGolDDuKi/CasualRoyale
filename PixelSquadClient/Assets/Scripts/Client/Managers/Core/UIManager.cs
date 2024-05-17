using UnityEngine;
using TMPro;
using System.Collections;
using System;
using UnityEngine.UI;

public class UIManager
{
    Transform _emotion;

    public void UpdatePopup(string message)
    {
        GameObject go = GenerateUI("UI/Popup");
        TMP_Text text = go.transform.GetComponentInChildren<TMP_Text>();
        text.text = message;
    }

    public HpBar GenerateHpBar(CreatureController cc)
    {
        GameObject go = GenerateUI("UI/HpBar", GameObject.Find("HpBars").transform);
        go.GetComponent<HpBar>().Init(cc);
        return go.GetComponent<HpBar>();
    }

    public Emotion GenerateEmotion(CreatureController cc)
    {
        if (_emotion == null)
            _emotion = GameObject.Find("Emotions").transform;

        GameObject go = GenerateUI("UI/Emotion", _emotion);
        go.GetComponent<Emotion>().Init(cc);
        return go.GetComponent<Emotion>();
    }

    public GameObject GenerateUI(string path, Transform parent = null)
    {
        GameObject go;
        if (!path.Contains("UI/"))
            path = $"UI/{path}";

        if (parent == null)
            go = Managers.Resource.Instantiate($"{path}", GameObject.Find("Canvas").transform);
        else
            go = Managers.Resource.Instantiate($"{path}", parent);

        return go;
    }

    //public GameObject GenerateUI(string path, Vector2 position, Transform parent = null)
    //{
    //    GameObject go;
    //    if (!path.Contains("UI/"))
    //        path = $"UI/{path}";

    //    if (parent == null)
    //    {
    //        go = Managers.Resource.Instantiate($"{path}", GameObject.Find("Canvas").transform);
    //        go.GetComponent<RectTransform>().anchoredPosition = position;
    //    }
    //    else
    //    {
    //        go = Managers.Resource.Instantiate($"{path}", parent);
    //        go.GetComponent<RectTransform>().anchoredPosition = position;
    //    }

    //    return go;
    //}

    public IEnumerator CoFadeOut(GameObject target, Action action = null)
    {
        float time = 0f;
        float maxTime = 3.0f;

        GameObject go = target;

        while (time <= maxTime)
        {
            Color color = go.GetComponent<Image>().color;
            color.a = Mathf.Lerp(0f, 1f, time / maxTime);
            go.GetComponent<Image>().color = color;

            if (time == maxTime)
                break;

            time += Time.deltaTime;

            if (time > maxTime)
                time = maxTime;

            yield return null;
        }

        if (action != null)
            action.Invoke();

        yield break;
    }

    public IEnumerator CoFadeInText(GameObject target, Action action = null)
    {
        float maxTime = 3.0f;
        float time = maxTime;

        GameObject go = target;

        while (time >= 0)
        {
            Color color = go.GetComponent<TMP_Text>().color;
            color.a = Mathf.Lerp(0f, 1f, time / maxTime);
            go.GetComponent<TMP_Text>().color = color;

            if (time == 0)
                break;

            time -= Time.deltaTime;

            if (time <= 0)
                time = 0;

            yield return null;
        }

        if (action != null)
            action.Invoke();

        yield break;
    }

    public IEnumerator CoFadeIn(GameObject target, Action action = null)
    {
        float maxTime = 3.0f;
        float time = maxTime;

        GameObject go = target;

        while (time >= 0)
        {
            Color color = go.GetComponent<Image>().color;
            color.a = Mathf.Lerp(0f, 1f, time / maxTime);
            go.GetComponent<Image>().color = color;

            if (time == 0)
                break;

            time -= Time.deltaTime;

            if (time <= 0)
                time = 0;

            yield return null;
        }

        if (action != null)
            action.Invoke();

        yield break;
    }
}
