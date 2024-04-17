using Google.Protobuf;
using Google.Protobuf.Protocol;
using HostServer;
using HostServer.Game;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Host
{
    class PacketHandler
    {
        public static void CH_MoveHandler(PacketSession session, IMessage packet)
        {
            CH_Move movePacket = packet as CH_Move;
            ClientSession clientSession = session as ClientSession;

            Player player = clientSession.MyPlayer;
            if (player == null)
                return;

            GameRoom room = player.Room;
            if (room == null)
                return;

            //Debug.Log($"Player[{player.Id}] 이동 - [{(movePacket.PosInfo.PosX).ToString("0.00")}, {movePacket.PosInfo.PosY.ToString("0.00")}]");

            room.Push(room.HandleMove, player, movePacket);
        }

        public static void CH_AttackHandler(PacketSession session, IMessage packet)
        {
            CH_Attack attackPacket = packet as CH_Attack;
            ClientSession clientSession = session as ClientSession;

            Player player = clientSession.MyPlayer;
            if (player == null)
                return;

            GameRoom room = player.Room;
            if (room == null)
                return;

            room.Push(room.HandleAttack, player, attackPacket);
        }

        public static void SH_ConnectClientHandler(PacketSession session, IMessage packet)
        {
            SH_ConnectClient connectPacket = packet as SH_ConnectClient;

            UnityEngine.GameObject go = UnityEngine.GameObject.Find("Host");
            go.GetComponent<HostPlayer>().Connect(connectPacket.PublicIp, connectPacket.PrivateIp);
        }

        public static void CH_SkillDamageHandler(PacketSession session, IMessage packet)
        {
            CH_SkillDamage damagePacket = packet as CH_SkillDamage;
            ClientSession clientSession = session as ClientSession;

            Player player = clientSession.MyPlayer;
            if (player == null)
                return;

            GameRoom room = player.Room;
            if (room == null)
                return;

            room.Push(room.HandleSkillDamage, player, damagePacket);
        }

        public static void CH_SendClassHandler(PacketSession session, IMessage packet)
        {
            CH_SendClass classPacket = packet as CH_SendClass;
            ClientSession clientSession = session as ClientSession;
            
            Player player = clientSession.MyPlayer;
            if (player == null)
                return;

            GameRoom room = player.Room;
            if (room == null)
                return;

            {
                player.Class = classPacket.Job;

                player.Info.Name = $"Player_{player.Info.ObjectId}";
                player.Info.PosInfo.State = ActionState.Idle;
                player.Info.PosInfo.PosX = 0;
                player.Info.PosInfo.PosY = 0;

                player.Info.StatInfo.MaxHp = Managers.Data.ClassData[player.Class.ToString()].MaxHp;
                player.Info.StatInfo.Hp = player.Info.StatInfo.MaxHp;
                player.Info.StatInfo.Speed = Managers.Data.ClassData[player.Class.ToString()].Speed;
                player.Info.StatInfo.Damage = Managers.Data.ClassData[player.Class.ToString()].Damage;
                player.Info.StatInfo.FirstSkillId = Managers.Data.ClassData[player.Class.ToString()].FirstSkillId;
                player.Info.StatInfo.SecondSkillId = Managers.Data.ClassData[player.Class.ToString()].SecondSkillId;
            }

            room.Push(room.EnterGame, player);
        }

        public static void CH_UseSkillHandler(PacketSession session, IMessage packet)
        {
            CH_UseSkill skillPacket = packet as CH_UseSkill;
            ClientSession clientSession = session as ClientSession;

            Player player = clientSession.MyPlayer;
            if (player == null)
                return;

            GameRoom room = player.Room;
            if (room == null)
                return;

            room.Push(room.HandleSkill, player, skillPacket);
        }

        public static void CH_ExitRoomHandler(PacketSession session, IMessage packet)
        {
            throw new NotImplementedException();
        }
    }
}