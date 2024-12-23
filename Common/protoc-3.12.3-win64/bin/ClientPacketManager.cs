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
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SAttack, MakePacket<S_Attack>);
		_handler.Add((ushort)MsgId.SAttack, PacketHandler.S_AttackHandler);		
		_onRecv.Add((ushort)MsgId.SChangeHp, MakePacket<S_ChangeHp>);
		_handler.Add((ushort)MsgId.SChangeHp, PacketHandler.S_ChangeHpHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);		
		_onRecv.Add((ushort)MsgId.SRejectLogin, MakePacket<S_RejectLogin>);
		_handler.Add((ushort)MsgId.SRejectLogin, PacketHandler.S_RejectLoginHandler);		
		_onRecv.Add((ushort)MsgId.SAcceptLogin, MakePacket<S_AcceptLogin>);
		_handler.Add((ushort)MsgId.SAcceptLogin, PacketHandler.S_AcceptLoginHandler);		
		_onRecv.Add((ushort)MsgId.SRoomList, MakePacket<S_RoomList>);
		_handler.Add((ushort)MsgId.SRoomList, PacketHandler.S_RoomListHandler);		
		_onRecv.Add((ushort)MsgId.SRejectEnter, MakePacket<S_RejectEnter>);
		_handler.Add((ushort)MsgId.SRejectEnter, PacketHandler.S_RejectEnterHandler);		
		_onRecv.Add((ushort)MsgId.SAcceptEnter, MakePacket<S_AcceptEnter>);
		_handler.Add((ushort)MsgId.SAcceptEnter, PacketHandler.S_AcceptEnterHandler);		
		_onRecv.Add((ushort)MsgId.SAcceptMake, MakePacket<S_AcceptMake>);
		_handler.Add((ushort)MsgId.SAcceptMake, PacketHandler.S_AcceptMakeHandler);		
		_onRecv.Add((ushort)MsgId.SRejectMake, MakePacket<S_RejectMake>);
		_handler.Add((ushort)MsgId.SRejectMake, PacketHandler.S_RejectMakeHandler);		
		_onRecv.Add((ushort)MsgId.SWinner, MakePacket<S_Winner>);
		_handler.Add((ushort)MsgId.SWinner, PacketHandler.S_WinnerHandler);		
		_onRecv.Add((ushort)MsgId.SUseSkill, MakePacket<S_UseSkill>);
		_handler.Add((ushort)MsgId.SUseSkill, PacketHandler.S_UseSkillHandler);		
		_onRecv.Add((ushort)MsgId.SEndGame, MakePacket<S_EndGame>);
		_handler.Add((ushort)MsgId.SEndGame, PacketHandler.S_EndGameHandler);
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