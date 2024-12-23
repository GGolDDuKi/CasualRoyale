using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterGamePacket = (S_EnterGame)packet;
        Managers.Game.Authority = enterGamePacket.Auth;
        GameObject go;

        switch (Managers.Game.Authority)
        {
            case Authority.Client:
                go = GameObject.Find("ReadyButton");
                if (go == null)
                    Managers.UI.GenerateUI("ReadyButton");
                break;
            case Authority.Host:
                go = GameObject.Find("StartButton");
                if (go == null)
                    Managers.UI.GenerateUI("StartButton");
                break;
        }

        Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGameHandler = (S_LeaveGame)packet;
        Managers.Object.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
        foreach (ObjectInfo obj in spawnPacket.Objects)
        {
            Managers.Object.Add(obj, myPlayer: false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = (S_Despawn)packet;

        foreach (int id in despawnPacket.ObjectIds)
        {
            GameObjectType objectType = ObjectManager.GetObjectTypeById(id);
            if (objectType == GameObjectType.Projectile)
            {
                GameObject go = Managers.Object.FindById(id);
                go.GetComponent<ProjectileController>().Destroy();
            }
            else
                Managers.Object.Remove(id);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = (S_Move)packet;

        UnityEngine.GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
            return;

        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
            return;

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return;

        bc.PosInfo = movePacket.PosInfo;
    }

    public static void S_RejectLoginHandler(PacketSession session, IMessage packet)
    {
        S_RejectLogin rejectPacket = (S_RejectLogin)packet;

        switch (rejectPacket.Reason)
        {
            case RejectionReason.SameName:
                Managers.UI.UpdatePopup("Nickname is duplicated.");
                break;
            case RejectionReason.BadName:
                Managers.UI.UpdatePopup("Nickname is inappropriate.");
                break;
            case RejectionReason.NoSpecialChar:
                Managers.UI.UpdatePopup("Special characters cannot be used.");
                break;
            default:
                Managers.UI.UpdatePopup("You were rejected.");
                break;
        }
    }

    public static void S_AcceptLoginHandler(PacketSession session, IMessage packet)
    {
        S_AcceptLogin loginPacket = packet as S_AcceptLogin;

        Managers.User.Info = loginPacket.UserInfo;

        Managers.Scene.LoadScene(Define.Scene.Lobby);
    }

    public static void HC_StartGameHandler(PacketSession session, IMessage packet)
    {
        HC_StartGame startPacket = packet as HC_StartGame;
        GameObject go;

        foreach (var info in startPacket.Players)
        {
            go = Managers.Object.FindById(info.ObjectId);
            go.GetComponent<PlayerController>().Pos = new Vector2(info.PosInfo.PosX, info.PosInfo.PosY);
            go.transform.position = go.GetComponent<PlayerController>().Pos;
        }

        switch (Managers.Game.Authority)
        {
            case Authority.Client:
                go = GameObject.Find("ReadyButton");
                if (go != null)
                    Managers.Resource.Destroy(go);
                break;
            case Authority.Host:
                go = GameObject.Find("StartButton");
                if (go != null)
                    Managers.Resource.Destroy(go);
                break;
        }

        GameObject ui = Managers.UI.GenerateUI("GameStartNotice");
        ui.GetComponent<MonoBehaviour>().StartCoroutine(Managers.UI.CoFadeInText(ui, () => { Managers.Resource.Destroy(ui); }));
    }

    public static void HC_EmoteHandler(PacketSession session, IMessage packet)
    {
        HC_Emote emotePacket = packet as HC_Emote;

        UnityEngine.GameObject go = Managers.Object.FindById(emotePacket.Id);
        if (go == null)
            return;

        if (Managers.Object.MyPlayer.Id == emotePacket.Id)
            return;

        PlayerController pc = go.GetComponent<PlayerController>();
        if (pc == null)
            return;

        Emotion emotion = Managers.UI.GenerateEmotion(pc);
        emotion.SetEmotion(emotePacket.EmoteName);
        emotion.GetComponent<MonoBehaviour>().StartCoroutine(Managers.UI.CoFadeIn(emotion.gameObject, () => { Managers.Resource.Destroy(emotion.gameObject); }));
    }

    public static void S_RoomListHandler(PacketSession session, IMessage packet)
    {
        if (Managers.Game.InGame == true)
            return;

        S_RoomList roomList = packet as S_RoomList;

        //기존에 없던 방 추가, 없어진 방 삭제
        {
            foreach (var room in roomList.Rooms)
            {
                if (!Managers.Room.RoomInfo.ContainsKey(room.RoomId))
                {
                    Managers.Room.RoomInfo.Add(room.RoomId, room);
                }
                else
                {
                    Managers.Room.RoomInfo[room.RoomId] = room;
                    Managers.Room.Room[room.RoomId].Init(Managers.Room.RoomInfo[room.RoomId]);
                }
            }

            List<int> removeKeys = new List<int>();
            bool contain = false;
            foreach (var room in Managers.Room.RoomInfo.Values)
            {
                foreach (var info in roomList.Rooms)
                {
                    if (room.RoomId == info.RoomId)
                    {
                        contain = true;
                        break;
                    }
                }

                if (contain == false)
                    removeKeys.Add(room.RoomId);

                contain = false;
            }

            foreach (var key in removeKeys)
            {
                Managers.Room.RoomInfo.Remove(key);
            }
        }

        if (Managers.Room.UpdateRoomList() == false)
        {
            return;
        }
    }

    public static void HC_CanStartHandler(PacketSession session, IMessage packet)
    {
        HC_CanStart startPacket = packet as HC_CanStart;

        GameObject go = GameObject.Find("StartButton");
        go.GetComponent<StartButton>().ChangeState(startPacket.Start);
    }

    public static void HC_HostDisconnectHandler(PacketSession session, IMessage packet)
    {
        if (Managers.Scene.CurrentScene.SceneType == Define.Scene.Game && Managers.Game.InGame == true)
        {
            Managers.UI.GenerateUI("UI/MissingHost");
        }
    }

    public static void HC_MissingHostHandler(PacketSession session, IMessage packet)
    {
        Managers.UI.GenerateUI("UI/MissingHost");
    }

    public static void S_EndGameHandler(PacketSession session, IMessage packet)
    {
        Managers.Object.MyPlayer.EndGame();
    }

    public static void S_WinnerHandler(PacketSession session, IMessage packet)
    {
        S_Winner winPacket = packet as S_Winner;
        Managers.Game.Rank = winPacket.Rank;
    }

    public static void S_RejectMakeHandler(PacketSession session, IMessage packet)
    {
        S_RejectMake rejectPacket = packet as S_RejectMake;

        switch (rejectPacket.Reason)
        {
            case RejectionReason.SameName:
                Managers.UI.UpdatePopup("Room title is duplicated.");
                break;
            case RejectionReason.BadName:
                Managers.UI.UpdatePopup("Room title is inappropriate.");
                break;
            case RejectionReason.NoSpecialChar:
                Managers.UI.UpdatePopup("Special characters cannot be used.");
                break;
            default:
                Managers.UI.UpdatePopup("You were rejected.");
                break;
        }
    }

    public static void S_AcceptMakeHandler(PacketSession session, IMessage packet)
    {
        S_AcceptMake acceptPacket = (S_AcceptMake)packet;
        Managers.Scene.LoadScene(Define.Scene.Game);
    }

    public static void S_AcceptEnterHandler(PacketSession session, IMessage packet)
    {
        S_AcceptEnter acceptPacket = (S_AcceptEnter)packet;

        Managers.Scene.LoadScene(Define.Scene.Game);
    }

    public static void S_RejectEnterHandler(PacketSession session, IMessage packet)
    {
        S_RejectEnter rejectPacket = packet as S_RejectEnter;

        switch (rejectPacket.Reason)
        {
            case RejectionReason.IncorrectPassword:
                Managers.UI.UpdatePopup("Password is incorrect.");
                break;
            case RejectionReason.FullRoom:
                Managers.UI.UpdatePopup("Room is full.");
                break;
            default:
                Managers.UI.UpdatePopup("You were rejected.");
                break;
        }
    }

    public static void S_AttackHandler(PacketSession session, IMessage packet)
    {
        S_Attack shootPacket = (S_Attack)packet;

        UnityEngine.GameObject go = Managers.Object.FindById(shootPacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.Attack();
        }
    }

    public static void S_UseSkillHandler(PacketSession session, IMessage packet)
    {
        S_UseSkill skillPacket = (S_UseSkill)packet;

        UnityEngine.GameObject go = Managers.Object.FindById(skillPacket.PlayerId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.UsingSkill(skillPacket.SkillId);
        }
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp changePacket = (S_ChangeHp)packet;

        UnityEngine.GameObject go = Managers.Object.FindById(changePacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.Hit(changePacket.Hp);
        }
    }

    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = (S_Die)packet;

        UnityEngine.GameObject go = Managers.Object.FindById(diePacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.Hp = 0;
            cc._hpBar.SetHpBar(cc.Hp, cc.MaxHp);
            cc.Die(diePacket);
        }
    }
}