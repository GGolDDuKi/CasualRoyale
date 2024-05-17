using Google.Protobuf;
using Google.Protobuf.Protocol;
using HostServer;
using HostServer.Game;
using ServerCore;
using System;

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
            //Managers.Network.StartListen();
            go.GetComponent<HostPlayer>().Connect(connectPacket.PublicIp, connectPacket.PrivateIp, connectPacket.Port, true, 3);
        }

        public static void CH_SkillEffectHandler(PacketSession session, IMessage packet)
        {
            CH_SkillEffect effectPacket = packet as CH_SkillEffect;
            ClientSession clientSession = session as ClientSession;

            Player player = clientSession.MyPlayer;
            if (player == null)
                return;

            GameRoom room = player.Room;
            if (room == null)
                return;

            Projectile projectile = HostServer.Game.ObjectManager.Instance.Add<Projectile>();
            {
                projectile.Owner = player;
                projectile.SkillId = effectPacket.SkillId;
                projectile.PosInfo.PosX = effectPacket.PosInfo.PosX;
                projectile.PosInfo.PosY = effectPacket.PosInfo.PosY;
                projectile.PosInfo.LastDirX = effectPacket.PosInfo.LastDirX;
                projectile.PosInfo.LastDirY = effectPacket.PosInfo.LastDirY;
                projectile.StartPos = projectile.Pos;
                projectile.DestPos = projectile.Pos + (projectile.LastDir * Managers.Data.SkillData[projectile.SkillId].Range);
                projectile.Name = Managers.Data.SkillData[projectile.SkillId].SkillName;
                projectile.Room = room;
            }

            room.Push(room.EnterGame, projectile);
        }

        public static void CH_EmoteHandler(PacketSession session, IMessage packet)
        {
            CH_Emote emotePacket = packet as CH_Emote;
            ClientSession clientSession = session as ClientSession;

            Player player = clientSession.MyPlayer;
            if (player == null)
                return;

            GameRoom room = player.Room;
            if (room == null)
                return;

            room.Push(room.HandleEmotion, player, emotePacket);
        }

        public static void CH_StartGameHandler(PacketSession session, IMessage packet)
        {
            CH_StartGame startPacket = packet as CH_StartGame;
            ClientSession clientSession = session as ClientSession;

            Player player = clientSession.MyPlayer;
            if (player == null)
                return;

            GameRoom room = player.Room;
            if (room == null)
                return;

            room.Push(room.InitGame);
        }

        public static void CH_ReadyHandler(PacketSession session, IMessage packet)
        {
            CH_Ready readyPacket = packet as CH_Ready;
            ClientSession clientSession = session as ClientSession;

            Player player = clientSession.MyPlayer;
            if (player == null)
                return;

            GameRoom room = player.Room;
            if (room == null)
                return;

            room.Push(room.UpdateReadyState, player, readyPacket);
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

        public static void CH_SendInfoHandler(PacketSession session, IMessage packet)
        {
            CH_SendInfo infoPacket = packet as CH_SendInfo;
            ClientSession clientSession = session as ClientSession;
            
            Player player = clientSession.MyPlayer;
            if (player == null)
                return;

            GameRoom room = player.Room;
            if (room == null)
                return;

            {
                player.Class = infoPacket.Job;

                player.Info.Name = $"{infoPacket.Name}";
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
            ClientSession clientSession = session as ClientSession;

            Player player = clientSession.MyPlayer;
            if (player == null)
                return;

            SessionManager.Instance.DisconnectSession(clientSession.SessionId);
        }
    }
}