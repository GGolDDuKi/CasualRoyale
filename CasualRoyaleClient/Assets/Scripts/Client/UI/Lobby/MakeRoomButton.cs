using UnityEngine;

public class MakeRoomButton : Button
{
    public override void OnClick()
    {
        Transform inputRoomInfo = GameObject.Find("InputRoomInfo").transform;
        inputRoomInfo.GetChild(0).gameObject.SetActive(true);
    }
}
