using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HpBar : MonoBehaviour
{
    CreatureController cc;
    RectTransform rect;
    Image fill;
    TMP_Text objectName;

    private void Update()
    {
        if(cc != null)
        {
            rect.position = new Vector2(cc.transform.position.x, cc.transform.position.y + 1.5f);
        }
    }

    public void Init(CreatureController cc)
    {
        Canvas canvas = transform.parent.parent.GetComponent<Canvas>();
        if(canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;

        this.cc = cc;
        rect = GetComponent<RectTransform>();
        fill = transform.GetChild(0).GetComponent<Image>();
        objectName = transform.GetChild(1).GetComponent<TMP_Text>();
        objectName.text = cc.Name;
    }

    public void SetHpBar(float curHp, float maxHp)
    {
        float ratio = curHp / maxHp;
        fill.fillAmount = ratio;
    }
}
