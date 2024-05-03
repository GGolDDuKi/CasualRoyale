using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenOptionButton : Button
{
    public override void OnClick()
    {
        base.OnClick();

        Managers.UI.GenerateUI("UI/Option");
    }
}
