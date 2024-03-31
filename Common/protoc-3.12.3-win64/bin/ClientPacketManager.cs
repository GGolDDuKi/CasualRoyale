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
		_onRecv.Add((ushort)MsgId.HcEnterGame, MakePacket<HC_EnterGame>);
		_handler.Add((ushort)MsgId.HcEnterGame, PacketHandler.HC_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.HcLeaveGame, MakePacket<HC_LeaveGame>);
		_handler.Add((ushort)MsgId.HcLeaveGame, PacketHandler.HC_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.HcSpawn, MakePacket<HC_Spawn>);
		_handler.Add((ushort)MsgId.HcSpawn, PacketHandler.HC_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.HcDespawn, MakePacket<HC_Despawn>);
		_handler.Add((ushort)MsgId.HcDespawn, PacketHandler.HC_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.HcMove, MakePacket<HC_Move>);
		_handler.Add((ushort)MsgId.HcMove, PacketHandler.HC_MoveHandler);		
		_onRecv.Add((ushort)MsgId.HcShoot, MakePacket<HC_Shoot>);
		_handler.Add((ushort)MsgId.HcShoot, PacketHandler.HC_ShootHandler);		
		_onRecv.Add((ushort)MsgId.HcChangeHp, MakePacket<HC_ChangeHp>);
		_handler.Add((ushort)MsgId.HcChangeHp, PacketHandler.HC_ChangeHpHandler);		
		_onRecv.Add((ushort)MsgId.HcDie, MakePacket<HC_Die>);
		_handler.Add((ushort)MsgId.HcDie, PacketHandler.HC_DieHandler);		
		_onRecv.Add((ushort)MsgId.ScRejectLogin, MakePacket<SC_RejectLogin>);
		_handler.Add((ushort)MsgId.ScRejectLogin, PacketHandler.SC_RejectLoginHandler);		
		_onRecv.Add((ushort)MsgId.ScAcceptLogin, MakePacket<SC_AcceptLogin>);
		_handler.Add((ushort)MsgId.ScAcceptLogin, PacketHandler.SC_AcceptLoginHandler);		
		_onRecv.Add((ushort)MsgId.ScRoomList, MakePacket<SC_RoomList>);
		_handler.Add((ushort)MsgId.ScRoomList, PacketHandler.SC_RoomListHandler);		
		_onRecv.Add((ushort)MsgId.ScRejectEnter, MakePacket<SC_RejectEnter>);
		_handler.Add((ushort)MsgId.ScRejectEnter, PacketHandler.SC_RejectEnterHandler);		
		_onRecv.Add((ushort)MsgId.ScAcceptEnter, MakePacket<SC_AcceptEnter>);
		_handler.Add((ushort)MsgId.ScAcceptEnter, PacketHandler.SC_AcceptEnterHandler);		
		_onRecv.Add((ushort)MsgId.ScAcceptMake, MakePacket<SC_AcceptMake>);
		_handler.Add((ushort)MsgId.ScAcceptMake, PacketHandler.SC_AcceptMakeHandler);		
		_onRecv.Add((ushort)MsgId.ScRejectMake, MakePacket<SC_RejectMake>);
		_handler.Add((ushort)MsgId.ScRejectMake, PacketHandler.SC_RejectMakeHandler);
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