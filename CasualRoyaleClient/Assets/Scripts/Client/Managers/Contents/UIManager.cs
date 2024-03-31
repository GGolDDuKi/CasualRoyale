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

    public GameObject GenerateUI(string path)
    {
        GameObject go = Managers.Resource.Instantiate(path, GameObject.Find("Canvas").transform);
        return go;
    }
}
