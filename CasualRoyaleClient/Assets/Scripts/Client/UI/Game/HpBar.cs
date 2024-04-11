using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    CreatureController cc;
    RectTransform rect;
    [SerializeField] Image fill;

    private void Update()
    {
        rect.position = new Vector2(cc.transform.position.x, cc.transform.position.y + 1.5f);
    }

    public void Init(CreatureController cc)
    {
        Canvas canvas = transform.parent.parent.GetComponent<Canvas>();
        if(canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;

        this.cc = cc;
        rect = GetComponent<RectTransform>();
        fill = transform.GetChild(0).GetComponent<Image>();
    }

    public void SetHpBar(float curHp, float maxHp)
    {
        float ratio = curHp / maxHp;
        fill.fillAmount = ratio;
    }
}
