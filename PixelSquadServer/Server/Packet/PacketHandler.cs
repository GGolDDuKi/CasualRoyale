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
    public static void C_LoginHandler(PacketSession session, IMessage packet)
    {
        C_Login loginPacket = (C_Login)packet;
        ClientSession clientSession = (ClientSession)session;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Program.Lobby.Push(Program.Lobby.EnterLobby, user, loginPacket);
    }

    public static void C_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        C_EnterRoom enterPacket = (C_EnterRoom)packet;
        ClientSession clientSession = (ClientSession)session;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Program.Lobby.Push(Program.Lobby.EnterRoom, user, enterPacket);
    }

    public static void C_MakeRoomHandler(PacketSession session, IMessage packet)
    {
        //TODO : 룸 만들고 아이디 할당, 받은 정보 입력 후 로비 _rooms에 저장
        C_MakeRoom roomPacket = (C_MakeRoom)packet;
        ClientSession clientSession = (ClientSession)session;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Program.Lobby.Push(Program.Lobby.AddRoom, user, roomPacket);
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

        if(leavePacket.Authority == Authority.Host)
            Program.Lobby.Push(Program.Lobby.RemoveRoom, player);
        else
            Program.Lobby.Push(Program.Lobby.LeaveRoom, player);

        Program.Lobby.Push(Program.Lobby.ChangeUserState, user, UserState.Lobby);
    }

    public static void C_EnterGameHandler(PacketSession session, IMessage packet)
    {
        C_EnterGame updatePacket = (C_EnterGame)packet;
        ClientSession clientSession = (ClientSession)session;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        Program.Lobby.Push(Program.Lobby.ChangeUserState, user, UserState.Game);
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

    public static void C_SkillEffectHandler(PacketSession session, IMessage packet)
    {
        C_SkillEffect effectPacket = (C_SkillEffect)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        Projectile projectile = ObjectManager.Instance.Add<Projectile>();
        {
            projectile.Owner = player;
            projectile.SkillId = effectPacket.SkillId;
            projectile.PosInfo.PosX = effectPacket.PosInfo.PosX;
            projectile.PosInfo.PosY = effectPacket.PosInfo.PosY;
            projectile.PosInfo.LastDirX = effectPacket.PosInfo.LastDirX;
            projectile.PosInfo.LastDirY = effectPacket.PosInfo.LastDirY;
            projectile.StartPos = projectile.Pos;
            projectile.DestPos = projectile.Pos + (projectile.LastDir * DataManager.Instance.GetSkillData(projectile.SkillId).Range);
            projectile.Name = DataManager.Instance.GetSkillData(projectile.SkillId).SkillName;
            projectile.Room = room;
        }

        room.Push(room.EnterGame, projectile);
    }

    public static void C_EmoteHandler(PacketSession session, IMessage packet)
    {
        C_Emote emotePacket = (C_Emote)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleEmotion, player, emotePacket);
    }

    public static void C_StartGameHandler(PacketSession session, IMessage packet)
    {
        C_StartGame startPacket = (C_StartGame)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.InitGame);
    }

    public static void C_ReadyHandler(PacketSession session, IMessage packet)
    {
        C_Ready readyPacket = (C_Ready)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.UpdateReadyState, player, readyPacket);
    }

    public static void C_SkillDamageHandler(PacketSession session, IMessage packet)
    {
        C_SkillDamage damagePacket = (C_SkillDamage)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleSkillDamage, player, damagePacket);
    }

    public static void C_SendInfoHandler(PacketSession session, IMessage packet)
    {
        C_SendInfo infoPacket = (C_SendInfo)packet;
        ClientSession clientSession = (ClientSession)session;

        User user = clientSession.MyUser;
        if (user == null)
            return;

        GameRoom room = user.Room;
        if (room == null)
            return;

        Player player = ObjectManager.Instance.Add<Player>();
        {
            player.Room = room;
            player.Class = infoPacket.Job;
            player.Session = clientSession;
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
        clientSession.MyPlayer = player;

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
