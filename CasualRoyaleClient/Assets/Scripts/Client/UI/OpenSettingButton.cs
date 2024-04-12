using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSettingButton : Button
{
    public override void OnClick()
    {
        Managers.UI.GenerateUI("UI/Setting");
    }
}
