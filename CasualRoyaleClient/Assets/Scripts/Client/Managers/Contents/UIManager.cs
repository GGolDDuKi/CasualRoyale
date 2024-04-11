using UnityEngine;
using TMPro;

public class UIManager
{
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

    public GameObject GenerateUI(string path, Transform parent = null)
    {
        GameObject go;

        if (parent == null)
            go = Managers.Resource.Instantiate(path, GameObject.Find("Canvas").transform);
        else
            go = Managers.Resource.Instantiate(path, parent);

        return go;
    }
}
