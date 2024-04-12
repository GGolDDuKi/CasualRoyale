using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CreatureController
{
    protected override IEnumerator CoDie(HC_Die diePacket)
    {
        Managers.Object.RemovePlayer(this.gameObject);

        if(Managers.Object.Players.Count > 0)
        {
            if (Camera.main.transform.parent == this.transform)
            {
                Managers.Game.CameraIndex = 0;
                Camera.main.transform.SetParent(Managers.Object.Players[Managers.Game.CameraIndex].transform);
                Camera.main.transform.localPosition = new Vector3(0, 0, -10);
            }
        }
        else
        {
            GameObject.Find("MonitorUI").gameObject.SetActive(false);
            GameObject go = Managers.UI.GenerateUI("UI/GameEnd");
            go.GetComponent<GameEnd>().Exit();
        }

        State = ActionState.Dead;
        yield return new WaitForSeconds(3.0f);

        Clear();
    }
}
