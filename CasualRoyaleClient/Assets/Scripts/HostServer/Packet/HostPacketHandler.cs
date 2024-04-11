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

        public static void CH_ExitRoomHandler(PacketSession session, IMessage packet)
        {
            throw new NotImplementedException();
        }
    }
}