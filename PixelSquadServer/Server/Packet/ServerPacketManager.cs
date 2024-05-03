using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.HsUpdateRoom, MakePacket<HS_UpdateRoom>);
		_handler.Add((ushort)MsgId.HsUpdateRoom, PacketHandler.HS_UpdateRoomHandler);		
		_onRecv.Add((ushort)MsgId.HsStartGame, MakePacket<HS_StartGame>);
		_handler.Add((ushort)MsgId.HsStartGame, PacketHandler.HS_StartGameHandler);		
		_onRecv.Add((ushort)MsgId.HsEndGame, MakePacket<HS_EndGame>);
		_handler.Add((ushort)MsgId.HsEndGame, PacketHandler.HS_EndGameHandler);		
		_onRecv.Add((ushort)MsgId.CsLogin, MakePacket<CS_Login>);
		_handler.Add((ushort)MsgId.CsLogin, PacketHandler.CS_LoginHandler);		
		_onRecv.Add((ushort)MsgId.CsEnterRoom, MakePacket<CS_EnterRoom>);
		_handler.Add((ushort)MsgId.CsEnterRoom, PacketHandler.CS_EnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.CsMakeRoom, MakePacket<CS_MakeRoom>);
		_handler.Add((ushort)MsgId.CsMakeRoom, PacketHandler.CS_MakeRoomHandler);		
		_onRecv.Add((ushort)MsgId.CsStartGame, MakePacket<CS_StartGame>);
		_handler.Add((ushort)MsgId.CsStartGame, PacketHandler.CS_StartGameHandler);		
		_onRecv.Add((ushort)MsgId.CsEndGame, MakePacket<CS_EndGame>);
		_handler.Add((ushort)MsgId.CsEndGame, PacketHandler.CS_EndGameHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}