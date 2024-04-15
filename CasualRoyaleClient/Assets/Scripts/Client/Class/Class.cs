using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class : MonoBehaviour
{
    public virtual void ClickSkill(int skillId)
    {
        CH_UseSkill skillPacket = new CH_UseSkill();
        skillPacket.SkillId = skillId;
        Managers.Network.H_Send(skillPacket);
    }
}
