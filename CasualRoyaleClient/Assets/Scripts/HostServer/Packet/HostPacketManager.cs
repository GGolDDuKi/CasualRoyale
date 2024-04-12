using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

namespace Host
{
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
			_onRecv.Add((ushort)MsgId.ChMove, MakePacket<CH_Move>);
			_handler.Add((ushort)MsgId.ChMove, PacketHandler.CH_MoveHandler);
			_onRecv.Add((ushort)MsgId.ChAttack, MakePacket<CH_Attack>);
			_handler.Add((ushort)MsgId.ChAttack, PacketHandler.CH_AttackHandler);
			_onRecv.Add((ushort)MsgId.ChExitRoom, MakePacket<CH_ExitRoom>);
			_handler.Add((ushort)MsgId.ChExitRoom, PacketHandler.CH_ExitRoomHandler);
			_onRecv.Add((ushort)MsgId.ShConnectClient, MakePacket<SH_ConnectClient>);
			_handler.Add((ushort)MsgId.ShConnectClient, PacketHandler.SH_ConnectClientHandler);
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
}