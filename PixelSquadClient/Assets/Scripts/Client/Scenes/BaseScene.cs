using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

	void Awake()
	{
		Init();
	}

	protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";

        Screen.SetResolution(Managers.Setting.Width, Managers.Setting.Height, false);
        GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution = new Vector2(Managers.Setting.Width, Managers.Setting.Height);
    }

    public abstract void Clear();
}
