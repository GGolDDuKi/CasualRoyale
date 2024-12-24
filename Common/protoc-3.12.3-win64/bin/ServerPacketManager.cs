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
		_onRecv.Add((ushort)MsgId.CMove, MakePacket<C_Move>);
		_handler.Add((ushort)MsgId.CMove, PacketHandler.C_MoveHandler);		
		_onRecv.Add((ushort)MsgId.CAttack, MakePacket<C_Attack>);
		_handler.Add((ushort)MsgId.CAttack, PacketHandler.C_AttackHandler);		
		_onRecv.Add((ushort)MsgId.CLogin, MakePacket<C_Login>);
		_handler.Add((ushort)MsgId.CLogin, PacketHandler.C_LoginHandler);		
		_onRecv.Add((ushort)MsgId.CEnterRoom, MakePacket<C_EnterRoom>);
		_handler.Add((ushort)MsgId.CEnterRoom, PacketHandler.C_EnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.CMakeRoom, MakePacket<C_MakeRoom>);
		_handler.Add((ushort)MsgId.CMakeRoom, PacketHandler.C_MakeRoomHandler);		
		_onRecv.Add((ushort)MsgId.CUseSkill, MakePacket<C_UseSkill>);
		_handler.Add((ushort)MsgId.CUseSkill, PacketHandler.C_UseSkillHandler);		
		_onRecv.Add((ushort)MsgId.CSendInfo, MakePacket<C_SendInfo>);
		_handler.Add((ushort)MsgId.CSendInfo, PacketHandler.C_SendInfoHandler);		
		_onRecv.Add((ushort)MsgId.CSkillEffect, MakePacket<C_SkillEffect>);
		_handler.Add((ushort)MsgId.CSkillEffect, PacketHandler.C_SkillEffectHandler);		
		_onRecv.Add((ushort)MsgId.CSkillDamage, MakePacket<C_SkillDamage>);
		_handler.Add((ushort)MsgId.CSkillDamage, PacketHandler.C_SkillDamageHandler);		
		_onRecv.Add((ushort)MsgId.CEnterGame, MakePacket<C_EnterGame>);
		_handler.Add((ushort)MsgId.CEnterGame, PacketHandler.C_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.CReady, MakePacket<C_Ready>);
		_handler.Add((ushort)MsgId.CReady, PacketHandler.C_ReadyHandler);		
		_onRecv.Add((ushort)MsgId.CStartGame, MakePacket<C_StartGame>);
		_handler.Add((ushort)MsgId.CStartGame, PacketHandler.C_StartGameHandler);		
		_onRecv.Add((ushort)MsgId.CEmote, MakePacket<C_Emote>);
		_handler.Add((ushort)MsgId.CEmote, PacketHandler.C_EmoteHandler);		
		_onRecv.Add((ushort)MsgId.CLeaveGame, MakePacket<C_LeaveGame>);
		_handler.Add((ushort)MsgId.CLeaveGame, PacketHandler.C_LeaveGameHandler);
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