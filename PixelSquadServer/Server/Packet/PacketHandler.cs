using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Data;
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

    public static void C_LoginHandler(PacketSession session, IMessage packet)
    {
        C_Login loginPacket = (C_Login)packet;
        ClientSession clientSession = (ClientSession)session;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        lobby.Push(lobby.EnterLobby, user, loginPacket);
    }

    public static void C_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        C_EnterRoom enterPacket = (C_EnterRoom)packet;
        ClientSession clientSession = (ClientSession)session;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        lobby.Push(lobby.EnterRoom, user, enterPacket);
    }

    public static void C_MakeRoomHandler(PacketSession session, IMessage packet)
    {
        //TODO : 룸 만들고 아이디 할당, 받은 정보 입력 후 로비 _rooms에 저장
        C_MakeRoom roomPacket = (C_MakeRoom)packet;
        ClientSession clientSession = (ClientSession)session;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        lobby.Push(lobby.AddRoom, user, roomPacket);
    }

    public static void C_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        C_LeaveGame leavePacket = (C_LeaveGame)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null) 
            return;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        if(leavePacket.Authority == Authority.Host)
            lobby.Push(lobby.RemoveRoom, player);

        lobby.Push(lobby.ChangeUserState, user, UserState.Lobby);
    }

    public static void CS_EnterGameHandler(PacketSession session, IMessage packet)
    {
        CS_EnterGame updatePacket = (CS_EnterGame)packet;
        ClientSession clientSession = (ClientSession)session;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Lobby lobby = user.Lobby;
        if (lobby == null)
            return;

        lobby.Push(lobby.ChangeUserState, user, UserState.Game);
    }

    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = (C_Move)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleMove, player, movePacket);
    }

    public static void C_AttackHandler(PacketSession session, IMessage packet)
    {
        C_Attack attackPacket = (C_Attack)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleAttack, player, attackPacket);
    }

    public static void CH_SkillEffectHandler(PacketSession session, IMessage packet)
    {
        CH_SkillEffect effectPacket = (CH_SkillEffect)packet;
        ClientSession clientSession = (ClientSession)session;

        //Player player = clientSession.MyPlayer;
        //if (player == null)
        //    return;

        //GameRoom room = player.Room;
        //if (room == null)
        //    return;

        //Projectile projectile = HostServer.Game.ObjectManager.Instance.Add<Projectile>();
        //{
        //    projectile.Owner = player;
        //    projectile.SkillId = effectPacket.SkillId;
        //    projectile.PosInfo.PosX = effectPacket.PosInfo.PosX;
        //    projectile.PosInfo.PosY = effectPacket.PosInfo.PosY;
        //    projectile.PosInfo.LastDirX = effectPacket.PosInfo.LastDirX;
        //    projectile.PosInfo.LastDirY = effectPacket.PosInfo.LastDirY;
        //    projectile.StartPos = projectile.Pos;
        //    projectile.DestPos = projectile.Pos + (projectile.LastDir * Managers.Data.SkillData[projectile.SkillId].Range);
        //    projectile.Name = Managers.Data.SkillData[projectile.SkillId].SkillName;
        //    projectile.Room = room;
        //}

        //room.Push(room.EnterGame, projectile);
    }

    public static void CH_EmoteHandler(PacketSession session, IMessage packet)
    {
        CH_Emote emotePacket = packet as CH_Emote;
        ClientSession clientSession = session as ClientSession;

        //Player player = clientSession.MyPlayer;
        //if (player == null)
        //    return;

        //GameRoom room = player.Room;
        //if (room == null)
        //    return;

        //room.Push(room.HandleEmotion, player, emotePacket);
    }

    public static void CH_StartGameHandler(PacketSession session, IMessage packet)
    {
        CH_StartGame startPacket = packet as CH_StartGame;
        ClientSession clientSession = session as ClientSession;

        //Player player = clientSession.MyPlayer;
        //if (player == null)
        //    return;

        //GameRoom room = player.Room;
        //if (room == null)
        //    return;

        //room.Push(room.InitGame);
    }

    public static void CH_ReadyHandler(PacketSession session, IMessage packet)
    {
        CH_Ready readyPacket = packet as CH_Ready;
        ClientSession clientSession = session as ClientSession;

        //Player player = clientSession.MyPlayer;
        //if (player == null)
        //    return;

        //GameRoom room = player.Room;
        //if (room == null)
        //    return;

        //room.Push(room.UpdateReadyState, player, readyPacket);
    }

    public static void CH_SkillDamageHandler(PacketSession session, IMessage packet)
    {
        CH_SkillDamage damagePacket = packet as CH_SkillDamage;
        ClientSession clientSession = session as ClientSession;

        //Player player = clientSession.MyPlayer;
        //if (player == null)
        //    return;

        //GameRoom room = player.Room;
        //if (room == null)
        //    return;

        //room.Push(room.HandleSkillDamage, player, damagePacket);
    }

    public static void C_SendInfoHandler(PacketSession session, IMessage packet)
    {
        C_SendInfo infoPacket = (C_SendInfo)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        {
            player.Class = infoPacket.Job;
            ClassData classData = DataManager.Instance.GetClassData(player.Class.ToString());

            player.Info.Name = $"{infoPacket.Name}";
            player.Info.PosInfo.State = ActionState.Idle;
            player.Info.PosInfo.PosX = 0;
            player.Info.PosInfo.PosY = 0;

            player.Info.StatInfo.MaxHp = classData.MaxHp;
            player.Info.StatInfo.Hp = player.Info.StatInfo.MaxHp;
            player.Info.StatInfo.Speed = classData.Speed;
            player.Info.StatInfo.Damage = classData.Damage;
            player.Info.StatInfo.FirstSkillId = classData.FirstSkillId;
            player.Info.StatInfo.SecondSkillId = classData.SecondSkillId;
        }

        room.Push(room.EnterGame, player);
    }

    public static void C_UseSkillHandler(PacketSession session, IMessage packet)
    {
        C_UseSkill skillPacket = (C_UseSkill)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleSkill, player, skillPacket);
    }
}
