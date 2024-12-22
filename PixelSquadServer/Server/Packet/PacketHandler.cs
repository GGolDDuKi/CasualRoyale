using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Object;
using Server.Room;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void HS_StartGameHandler(PacketSession session, IMessage packet)
    {
        throw new NotImplementedException();
    }

    public static void HS_EndGameHandler(PacketSession session, IMessage packet)
    {
        HS_EndGame endPacket = packet as HS_EndGame;
        ClientSession clientSession = session as ClientSession;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        lobby.Push(lobby.RemoveRoom, user, endPacket);
    }

    public static void CS_LoginHandler(PacketSession session, IMessage packet)
    {
        CS_Login loginPacket = packet as CS_Login;
        ClientSession clientSession = session as ClientSession;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        lobby.Push(lobby.EnterLobby, user, loginPacket);
    }

    public static void CS_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        CS_EnterRoom enterPacket = packet as CS_EnterRoom;
        ClientSession clientSession = session as ClientSession;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        lobby.Push(lobby.EnterRoom, user, enterPacket);
    }

    public static void CS_MakeRoomHandler(PacketSession session, IMessage packet)
    {
        //TODO : 룸 만들고 아이디 할당, 받은 정보 입력 후 로비 _rooms에 저장
        CS_MakeRoom roomPacket = packet as CS_MakeRoom;
        ClientSession clientSession = session as ClientSession;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        lobby.Push(lobby.AddRoom, user, roomPacket);
    }

    public static void CS_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        CS_LeaveGame updatePacket = packet as CS_LeaveGame;
        ClientSession clientSession = session as ClientSession;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        lobby.Push(lobby.ChangeUserState, user, UserState.Lobby);
    }

    public static void CS_EnterGameHandler(PacketSession session, IMessage packet)
    {
        CS_EnterGame updatePacket = packet as CS_EnterGame;
        ClientSession clientSession = session as ClientSession;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        lobby.Push(lobby.ChangeUserState, user, UserState.Game);
    }

    public static void HS_UpdateRoomHandler(PacketSession session, IMessage packet)
    {
        HS_UpdateRoom updatePacket = packet as HS_UpdateRoom;
        ClientSession clientSession = session as ClientSession;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        lobby.Push(lobby.UpdateRoom, updatePacket);
    }

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
