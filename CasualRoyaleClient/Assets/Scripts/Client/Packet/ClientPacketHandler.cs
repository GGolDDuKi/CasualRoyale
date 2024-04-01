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
        }

        public static void HC_LeaveGameHandler(PacketSession session, IMessage packet)
        {
            HC_LeaveGame leaveGameHandler = packet as HC_LeaveGame;
            Managers.Object.Clear();
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

            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc == null)
                return;

            cc.PosInfo = movePacket.PosInfo;
        }

        public static void SC_RejectLoginHandler(PacketSession session, IMessage packet)
        {
            Managers.UI.UpdatePopup("중복되는 닉네임입니다.");
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
                }

                List<int> removeKeys = new List<int>();
                foreach (var room in Managers.Room.RoomInfo.Values)
                {
                    if (!roomList.Rooms.Contains(room))
                    {
                        removeKeys.Add(room.RoomId);
                    }
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

        public static void SC_RejectMakeHandler(PacketSession session, IMessage packet)
        {
            Managers.UI.UpdatePopup("동일한 방 제목이 존재합니다.");
        }

        public static void SC_AcceptMakeHandler(PacketSession session, IMessage packet)
        {
            SC_AcceptMake acceptPacket = packet as SC_AcceptMake;
            Managers.Room.MyRoom = acceptPacket.Room;

            Managers.Scene.LoadScene(Define.Scene.Game);

            GameObject go = Managers.Resource.Instantiate("Host/Host");
            UnityEngine.Object.DontDestroyOnLoad(go);

            Managers.Network.Connect(Managers.User.PublicIp, "127.0.0.1");
        }

        public static void SC_AcceptEnterHandler(PacketSession session, IMessage packet)
        {
            SC_AcceptEnter acceptPacket = packet as SC_AcceptEnter;

            Managers.Scene.LoadScene(Define.Scene.Game);
            //TODD : 서버에서 보내준 호스트의 Ip로 연결시도
            //Managers.Network.Listen();
            Managers.Network.Connect(Managers.User.PublicIp, "127.0.0.1");
            //Managers.Network.Connect(acceptPacket.PublicIp, acceptPacket.PrivateIp);
        }

        public static void SC_RejectEnterHandler(PacketSession session, IMessage packet)
        {
            Managers.UI.UpdatePopup("비밀번호가 틀렸습니다.");
        }

        public static void HC_ShootHandler(PacketSession session, IMessage packet)
        {
            HC_Shoot shootPacket = packet as HC_Shoot;

            UnityEngine.GameObject go = Managers.Object.FindById(shootPacket.ObjectId);
            if (go == null)
                return;

        }

        public static void HC_ChangeHpHandler(PacketSession session, IMessage packet)
        {
            HC_ChangeHp changePacket = packet as HC_ChangeHp;

            UnityEngine.GameObject go = Managers.Object.FindById(changePacket.ObjectId);
            if (go == null)
                return;

            //CreatureController cc = go.GetComponent<CreatureController>();
            //if (cc != null)
            //{
            //    cc.Hp = changePacket.Hp;
            //}
        }

        public static void HC_DieHandler(PacketSession session, IMessage packet)
        {
            HC_Die diePacket = packet as HC_Die;

            UnityEngine.GameObject go = Managers.Object.FindById(diePacket.ObjectId);
            if (go == null)
                return;

            //CreatureController cc = go.GetComponent<CreatureController>();
            //if (cc != null)
            //{
            //    cc.Hp = 0;
            //    cc.OnDead();
            //}
        }
    }
}