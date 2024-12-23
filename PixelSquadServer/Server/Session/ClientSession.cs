using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Object;

namespace Server
{
	public class ClientSession : PacketSession
	{
		public User MyUser { get; set; }
		public Player MyPlayer { get; set; }
		public int SessionId { get; set; }

        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			MyUser = UserManager.Instance.Add<User>();
			{
				MyUser.JobType = JobType.Knight;
				MyUser.Session = this;
			}

			MyUser.Lobby = Program.Lobby;
			Program.Lobby.AddUser(MyUser);
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			Lobby lobby = LobbyManager.Instance.Find(1);
			lobby.Push(lobby.LeaveLobby, MyUser.Info.Id);

			SessionManager.Instance.Remove(this);

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{

		}

		public static string SecondCharToLower(string input)
		{
			if (string.IsNullOrEmpty(input))
				return "";
			return input[0] + input[1].ToString().ToLower() + input.Substring(2);
		}
	}
}
