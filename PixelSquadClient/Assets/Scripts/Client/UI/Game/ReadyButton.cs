using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Google.Protobuf.Protocol;

public class ReadyButton : Button
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
        ChangeState();
    }

    public override void OnClick()
    {
        base.OnClick();

        Managers.Game.Ready = !Managers.Game.Ready;
        ChangeState();

        //호스트로 준비 상태 전송
        C_Ready packet = new C_Ready();
        packet.Ready = Managers.Game.Ready;
        Managers.Network.Send(packet);
    }

    void ChangeState()
    {
        switch (Managers.Game.Ready)
        {
            case true:
                _text.text = "Cancel";
                _state.color = Color.green;
                break;
            case false:
                _text.text = "Ready";
                _state.color = Color.red;
                break;
        }
    }
}
