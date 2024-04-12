using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenOptionButton : Button
{
    public override void OnClick()
    {
        Managers.UI.GenerateUI("UI/Option");
    }
}
