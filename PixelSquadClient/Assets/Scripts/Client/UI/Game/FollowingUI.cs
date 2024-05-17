using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FollowingUI : MonoBehaviour
{
    CreatureController cc;
    RectTransform rect;

    public virtual void Init(CreatureController cc)
    {
        Canvas canvas = transform.parent.parent.GetComponent<Canvas>();
        if (canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;

        this.cc = cc;
        rect = GetComponent<RectTransform>();
    }

    public void FollowObject(float distance)
    {
        if (cc != null)
            rect.position = new Vector2(cc.transform.position.x, cc.transform.position.y + distance);
    }
}
