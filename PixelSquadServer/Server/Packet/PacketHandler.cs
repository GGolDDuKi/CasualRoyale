using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
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
}
