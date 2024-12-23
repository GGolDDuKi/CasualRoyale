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
            _onRecv.Add((ushort)MsgId.ChUseSkill, MakePacket<CH_UseSkill>);
            _handler.Add((ushort)MsgId.ChUseSkill, PacketHandler.CH_UseSkillHandler);
            _onRecv.Add((ushort)MsgId.ChSendInfo, MakePacket<CH_SendInfo>);
            _handler.Add((ushort)MsgId.ChSendInfo, PacketHandler.CH_SendInfoHandler);
            _onRecv.Add((ushort)MsgId.ChSkillEffect, MakePacket<CH_SkillEffect>);
            _handler.Add((ushort)MsgId.ChSkillEffect, PacketHandler.CH_SkillEffectHandler);
            _onRecv.Add((ushort)MsgId.ChSkillDamage, MakePacket<CH_SkillDamage>);
            _handler.Add((ushort)MsgId.ChSkillDamage, PacketHandler.CH_SkillDamageHandler);
            _onRecv.Add((ushort)MsgId.ChReady, MakePacket<CH_Ready>);
            _handler.Add((ushort)MsgId.ChReady, PacketHandler.CH_ReadyHandler);
            _onRecv.Add((ushort)MsgId.ChStartGame, MakePacket<CH_StartGame>);
            _handler.Add((ushort)MsgId.ChStartGame, PacketHandler.CH_StartGameHandler);
            _onRecv.Add((ushort)MsgId.ChEmote, MakePacket<CH_Emote>);
            _handler.Add((ushort)MsgId.ChEmote, PacketHandler.CH_EmoteHandler);
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
