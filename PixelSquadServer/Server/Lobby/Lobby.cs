using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Job;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	public enum UserState
	{
		None,
		Lobby,
		Game
	}

	public class Lobby : JobSerializer
	{
		public int LobbyId { get; set; }

		//TODO : 로비 유저랑 인게임 유저 분리해서 관리하면 좋을 듯?
		Dictionary<int, User> _lobbyUsers = new Dictionary<int, User>();
		public Dictionary<int, User> LobbyUsers { get { return _lobbyUsers; } }

		Dictionary<int, User> _gameUsers = new Dictionary<int, User>();
		public Dictionary<int, User> GameUsers { get { return _gameUsers; } }

		Dictionary<int, User> _users = new Dictionary<int, User>();
		public Dictionary<int, User> Users { get { return _users; } }

		Dictionary<int, GameRoomInfo> _rooms = new Dictionary<int, GameRoomInfo>();
		public Dictionary<int, GameRoomInfo> Rooms { get { return _rooms; } }

		int _roomCount = 0;

		public void Init()
		{

		}

		public void Update()
		{
            Flush();
        }

		public void AddUser(User user)
        {
			_users.Add(user.Id, user);
        }

		public void EnterLobby(User user, CS_Login packet)
		{
			if (user == null)
				return;

			foreach(var u in _users.Values)
            {
				if (packet.Info.Name == u.Name)
                {
					SC_RejectLogin rejectPacket = new SC_RejectLogin();
					rejectPacket.Reason = RejectionReason.SameName;
					user.Session.Send(rejectPacket);
					return;
                }
            }

			User newUser = _users[user.Id];
			newUser.Info.Name = packet.Info.Name;
			newUser.Info.PrivateIp = packet.Info.PrivateIp;
			newUser.Lobby = this;

			SC_AcceptLogin acceptPacket = new SC_AcceptLogin();
			acceptPacket.UserInfo = newUser.Info;
			user.Session.Send(acceptPacket);
			Console.WriteLine($"{acceptPacket.UserInfo.Name} 로그인");

			_lobbyUsers.Add(user.Id, user);

			UpdateRoomList(newUser);
		}

		public void LeaveLobby(int objectId)
		{
			GameRoomInfo room = null;
			foreach(var r in _rooms.Values)
            {
				if(r.HostId == objectId)
                {
					if (_rooms.Remove(r.Id, out room) == false)
						return;
                }
            }

			User user = null;
			if (_users.Remove(objectId, out user) == false)
				return;

			user.Lobby = null;
		}

		public void UpdateRoomList(User user = null)
        {
			SC_RoomList roomList = new SC_RoomList();
			foreach (GameRoomInfo room in _rooms.Values)
			{
				RoomInfo roomInfo = new RoomInfo();
				roomInfo.RoomId = room.Id;
				roomInfo.RoomName = room.Name;
				roomInfo.SecretRoom = room.SecretRoom;
				roomInfo.CurMember = room.CurMember;
				roomInfo.MaxMember = room.MaxMember;
				roomInfo.HostName = room.HostName;
				roomList.Rooms.Add(roomInfo);
			}

			if (user == null)
				Broadcast(roomList, UserState.Lobby);
			else
				user.Session.Send(roomList);
		}

		public void AddRoom(User user, CS_MakeRoom packet)
        {
			GameRoomInfo room = new GameRoomInfo();
			room.Info = packet.Room;

			foreach(GameRoomInfo r in _rooms.Values)
            {
				if (room.Info.RoomName == r.Name)
                {
					SC_RejectMake rejectPacket = new SC_RejectMake();
					rejectPacket.Reason = RejectionReason.SameName;
					user.Session.Send(rejectPacket);
                    Console.WriteLine($"{user.Name}님의 방 생성 거절");
					return;
                }
            }

			room.Id = _roomCount++;
			_rooms.Add(room.Id, room);
			SC_AcceptMake acceptPacket = new SC_AcceptMake();
			acceptPacket.Room = room.Info;
			user.Session.Send(acceptPacket);
			Console.WriteLine($"{user.Name}님이 [{room.Name}]방 생성");

			UpdateRoomList();
		}

		public void RemoveRoom(User user, HS_EndGame packet)
		{
			if (!_rooms.ContainsKey(packet.Room.RoomId))
				return;

			_rooms.Remove(packet.Room.RoomId);
			UpdateRoomList();
		}

		public void EnterRoom(User user, CS_EnterRoom packet)
        {
			GameRoomInfo room = _rooms[packet.RoomId];

			if(room.CurMember >= room.MaxMember)
            {
				SC_RejectEnter rejectPacket = new SC_RejectEnter();
				rejectPacket.Reason = RejectionReason.FullRoom;
				user.Session.Send(rejectPacket);
				return;
			}

			if(room.Password == packet.PassWord)
            {
				SC_AcceptEnter acceptPacket = new SC_AcceptEnter();
				acceptPacket.PublicIp = _users[room.HostId].PublicIp;
				acceptPacket.PrivateIp = _users[room.HostId].PrivateIp;
				acceptPacket.Port = _users[room.HostId].Port;

				SH_ConnectClient connectPacket = new SH_ConnectClient();
				connectPacket.PublicIp = user.PublicIp;
				connectPacket.PrivateIp = user.PrivateIp;
				connectPacket.Port = user.Port;

				user.Session.Send(acceptPacket);
				_users[room.HostId].Session.Send(connectPacket);

                Console.WriteLine($"{user.Name}님이 {room.Name}에 입장하셨습니다.");
            }
            else
            {
				SC_RejectEnter rejectPacket = new SC_RejectEnter();
				rejectPacket.Reason = RejectionReason.IncorrectPassword;
				user.Session.Send(rejectPacket);
            }
        }

		public void UpdateRoom(HS_UpdateRoom packet)
        {
			if (packet == null)
				return;

			_rooms[packet.Info.RoomId].Info = packet.Info;

			UpdateRoomList();
        }

		public void ChangeUserState(User user, UserState state = UserState.Lobby)
        {
            switch (state)
            {
				case UserState.Lobby:
					{
						User target;
						if (_gameUsers.TryGetValue(user.Id, out target))
						{
							_gameUsers.Remove(user.Id);
							_lobbyUsers.Add(user.Id, target);
						}
					}
					break;

				case UserState.Game:
                    {
						User target;
						if (_lobbyUsers.TryGetValue(user.Id, out target))
						{
							_lobbyUsers.Remove(user.Id);
							_gameUsers.Add(user.Id, target);
						}
					}
					break;
            }
        }

		public User FindPlayer(Func<User, bool> condition)
		{
			foreach (User user in _users.Values)
			{
				if (condition.Invoke(user))
					return user;
			}

			return null;
		}

		public void Broadcast(IMessage packet, UserState state = UserState.None)
		{
            switch (state)
            {
				case UserState.None:
					foreach (User p in _users.Values)
					{
						p.Session.Send(packet);
					}
					break;
				case UserState.Lobby:
					foreach (User p in _lobbyUsers.Values)
					{
						p.Session.Send(packet);
					}
					break;
				case UserState.Game:
					foreach (User p in _gameUsers.Values)
					{
						p.Session.Send(packet);
					}
					break;
			}
			
		}
	}
}
