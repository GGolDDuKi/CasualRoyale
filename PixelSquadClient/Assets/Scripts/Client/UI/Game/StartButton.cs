using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Google.Protobuf.Protocol;

public class StartButton : Button
{
    TMP_Text _text;
    Image _state;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        _text = transform.GetChild(0).GetComponent<TMP_Text>();
        _state = transform.GetChild(1).GetComponent<Image>();
        ChangeState(false);
    }

    public override void OnClick()
    {
        base.OnClick();
        
        //게임 시작 신호 전송
        C_StartGame startPacket = new C_StartGame();
        Managers.Network.Send(startPacket);
    }

    public void ChangeState(bool canStart)
    {
        switch (canStart)
        {
            case true:
                _text.text = "Start";
                _state.color = Color.green;
                GetComponent<UnityEngine.UI.Button>().enabled = true;
                break;
            case false:
                _text.text = "Wait";
                _state.color = Color.red;
                GetComponent<UnityEngine.UI.Button>().enabled = false;
                break;
        }
    }
}
