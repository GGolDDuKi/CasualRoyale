using UnityEngine;

public class MakeRoomButton : Button
{
    public override void OnClick()
    {
        base.OnClick();
        Transform inputRoomInfo = GameObject.Find("InputRoomInfo").transform;
        inputRoomInfo.GetChild(0).gameObject.SetActive(true);
    }
}
