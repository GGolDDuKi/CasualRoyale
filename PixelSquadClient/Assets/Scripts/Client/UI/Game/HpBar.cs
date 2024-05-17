using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HpBar : FollowingUI
{
    Image fill;

    private void Update()
    {
        FollowObject(1.5f);
    }

    public override void Init(CreatureController cc)
    {
        base.Init(cc);
        fill = transform.GetChild(0).GetComponent<Image>();
    }

    public void SetHpBar(float curHp, float maxHp)
    {
        float ratio = curHp / maxHp;
        fill.fillAmount = ratio;
    }
}
