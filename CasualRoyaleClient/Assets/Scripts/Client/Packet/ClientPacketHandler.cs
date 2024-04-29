using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Client
{
    class PacketHandler
    {
        public static void HC_EnterGameHandler(PacketSession session, IMessage packet)
        {
            HC_EnterGame enterGamePacket = packet as HC_EnterGame;
            Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
            Managers.Game.InGame = true;
        }

        public static void HC_LeaveGameHandler(PacketSession session, IMessage packet)
        {
            HC_LeaveGame leaveGameHandler = packet as HC_LeaveGame;
            Managers.Object.Clear();
            Managers.Game.InGame = false;
        }

        public static void HC_SpawnHandler(PacketSession session, IMessage packet)
        {
            HC_Spawn spawnPacket = packet as HC_Spawn;
            foreach (ObjectInfo obj in spawnPacket.Objects)
            {
                Managers.Object.Add(obj, myPlayer: false);
            }
        }

        public static void HC_DespawnHandler(PacketSession session, IMessage packet)
        {
            HC_Despawn despawnPacket = packet as HC_Despawn;

            foreach (int id in despawnPacket.ObjectIds)
            {
                GameObjectType objectType = ObjectManager.GetObjectTypeById(id);
                if(objectType == GameObjectType.Projectile)
                {
                    GameObject go = Managers.Object.FindById(id);
                    go.GetComponent<ProjectileController>().Destroy();
                }
                else
                    Managers.Object.Remove(id);
            }
        }

        public static void HC_MoveHandler(PacketSession session, IMessage packet)
        {
            HC_Move movePacket = packet as HC_Move;

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

        public static void SC_RejectLoginHandler(PacketSession session, IMessage packet)
        {
            SC_RejectLogin rejectPacket = packet as SC_RejectLogin;

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

        public static void SC_AcceptLoginHandler(PacketSession session, IMessage packet)
        {
            SC_AcceptLogin loginPacket = packet as SC_AcceptLogin;

            Managers.User.Info = loginPacket.UserInfo;

            Managers.Scene.LoadScene(Define.Scene.Lobby);
        }

        public static void SC_RoomListHandler(PacketSession session, IMessage packet)
        {
            SC_RoomList roomList = packet as SC_RoomList;

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
                        Managers.Room.Room[room.RoomId].Init(room);
                    }
                }

                List<int> removeKeys = new List<int>();
                bool contain = false;
                foreach (var room in Managers.Room.RoomInfo.Values)
                {
                    foreach(var info in roomList.Rooms)
                    {
                        if (room.RoomId == info.RoomId)
                        {
                            contain = true;
                            break;
                        }
                    }

                    if(contain == false)
                        removeKeys.Add(room.RoomId);

                    contain = false;
                }

                foreach (var key in removeKeys)
                {
                    Managers.Room.RoomInfo.Remove(key);
                }
            }

            if(Managers.Room.UpdateRoomList() == false)
            {
                return;
            }
        }

        public static void HC_RequestInfoHandler(PacketSession session, IMessage packet)
        {
            CH_SendInfo infoPacket = new CH_SendInfo();
            infoPacket.Job = Managers.User.Job;
            infoPacket.Name = Managers.User.Name;
            Managers.Network.H_Send(infoPacket);
        }

        public static void HC_MissingHostHandler(PacketSession session, IMessage packet)
        {
            Managers.UI.GenerateUI("UI/MissingHost");
        }

        public static void HC_EndGameHandler(PacketSession session, IMessage packet)
        {
            Managers.Object.MyPlayer.EndGame();
        }

        public static void HC_WinnerHandler(PacketSession session, IMessage packet)
        {
            HC_Winner winPacket = packet as HC_Winner;
            Managers.Game.Rank = winPacket.Rank;
        }

        public static void SC_RejectMakeHandler(PacketSession session, IMessage packet)
        {
            SC_RejectMake rejectPacket = packet as SC_RejectMake;

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

        public static void SC_AcceptMakeHandler(PacketSession session, IMessage packet)
        {
            SC_AcceptMake acceptPacket = packet as SC_AcceptMake;
            Managers.Room.MyRoom = acceptPacket.Room;
            Managers.Scene.LoadScene(Define.Scene.Game);

            GameObject go = GameObject.Find("Host");
            if(go == null)
                go = Managers.Resource.Instantiate("Host/Host");
            UnityEngine.Object.DontDestroyOnLoad(go);

            //Managers.Game.Host = true;

            Managers.Network.Connect(Managers.User.PublicIp, "127.0.0.1");
        }

        public static void SC_AcceptEnterHandler(PacketSession session, IMessage packet)
        {
            SC_AcceptEnter acceptPacket = packet as SC_AcceptEnter;

            Managers.Scene.LoadScene(Define.Scene.Game);
            //TODD : 서버에서 보내준 호스트의 Ip로 연결시도

            Managers.Network.Connect(Managers.User.PublicIp, "127.0.0.1");
            //Managers.Network.Connect(acceptPacket.PublicIp, acceptPacket.PrivateIp);
        }

        public static void SC_RejectEnterHandler(PacketSession session, IMessage packet)
        {
            SC_RejectEnter rejectPacket = packet as SC_RejectEnter;

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

        public static void HC_AttackHandler(PacketSession session, IMessage packet)
        {
            HC_Attack shootPacket = packet as HC_Attack;

            UnityEngine.GameObject go = Managers.Object.FindById(shootPacket.ObjectId);
            if (go == null)
                return;

            CreatureController cc = go.GetComponent<CreatureController>();
            if(cc != null)
            {
                cc.Attack();
            }
        }

        public static void HC_UseSkillHandler(PacketSession session, IMessage packet)
        {
            HC_UseSkill skillPacket = packet as HC_UseSkill;

            UnityEngine.GameObject go = Managers.Object.FindById(skillPacket.PlayerId);
            if (go == null)
                return;

            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
            {
                cc.UsingSkill(skillPacket.SkillId);
            }
        }

        public static void HC_ChangeHpHandler(PacketSession session, IMessage packet)
        {
            HC_ChangeHp changePacket = packet as HC_ChangeHp;

            UnityEngine.GameObject go = Managers.Object.FindById(changePacket.ObjectId);
            if (go == null)
                return;

            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
            {
                cc.Hit(changePacket.Hp);
            }
        }

        public static void HC_DieHandler(PacketSession session, IMessage packet)
        {
            HC_Die diePacket = packet as HC_Die;

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
}