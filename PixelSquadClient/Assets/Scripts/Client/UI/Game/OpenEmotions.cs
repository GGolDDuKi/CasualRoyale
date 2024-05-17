using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenEmotions : Button
{
    public override void OnClick()
    {
        base.OnClick();
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
