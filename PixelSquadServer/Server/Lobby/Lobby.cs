using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Job;
using Server.Object;
using Server.Room;
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

		Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
		public Dictionary<int, GameRoom> Rooms { get { return _rooms; } }

		int _roomCount = 0;

		public void Init()
		{

		}

		public override void Update()
		{
			base.Update();
        }

		public void AddUser(User user)
        {
			_users.Add(user.Id, user);
        }

		public void EnterLobby(User user, C_Login packet)
		{
			if (user == null)
				return;

			foreach(var u in _users.Values)
            {
				if (packet.Info.Name == u.Name)
                {
					S_RejectLogin rejectPacket = new S_RejectLogin();
					rejectPacket.Reason = RejectionReason.SameName;
					user.Session.Send(rejectPacket);
					return;
                }
            }

			User newUser = _users[user.Id];
			newUser.Info.Name = packet.Info.Name;
			newUser.Lobby = this;
			user = newUser;

            S_AcceptLogin acceptPacket = new S_AcceptLogin();
			acceptPacket.UserInfo = newUser.Info;
			user.Session.Send(acceptPacket);
			Console.WriteLine($"{acceptPacket.UserInfo.Name} 로그인");

			_lobbyUsers.Add(user.Id, user);

			UpdateRoomList(newUser);
		}

		public void LeaveLobby(int objectId)
		{
			GameRoom room = null;
			foreach(var r in _rooms.Values)
            {
				if(r.Info.HostId == objectId)
                {
					if (_rooms.Remove(r.Info.Id, out room) == false)
					{
						RoomManager.Instance.Remove(r.Info.Id);
						return;
					}
                }
            }

			User user = null;
			if (_users.Remove(objectId, out user) == false)
				return;

			user.Lobby = null;
		}

		public void UpdateRoomList(User user = null)
        {
			S_RoomList roomList = new S_RoomList();
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

		public void AddRoom(User user, C_MakeRoom packet)
        {
			GameRoomInfo room = new GameRoomInfo();
			room.Info = packet.Room;

			foreach(GameRoomInfo r in _rooms.Values)
            {
				if (room.Info.RoomName == r.Name)
                {
					S_RejectMake rejectPacket = new S_RejectMake();
					rejectPacket.Reason = RejectionReason.SameName;
					user.Session.Send(rejectPacket);
                    Console.WriteLine($"{user.Name}님의 방 생성 거절");
					return;
                }
            }

            GameRoom newRoom = RoomManager.Instance.Add();
			room.Id = newRoom.RoomId;

            _rooms.Add(newRoom.RoomId, room);
			S_AcceptMake acceptPacket = new S_AcceptMake();
			acceptPacket.Room = room.Info;
			user.Session.Send(acceptPacket);
			user.Room = newRoom;

			Console.WriteLine($"{user.Name}님이 [{room.Name}]방 생성");

			UpdateRoomList();
		}

		public void RemoveRoom(Player player)
		{
			if (!_rooms.ContainsKey(player.Room.RoomId))
				return;

			_rooms.Remove(player.Room.RoomId);
			UpdateRoomList();
		}

		public void EnterRoom(User user, C_EnterRoom packet)
        {
			GameRoomInfo room = _rooms[packet.RoomId];

			if(room.CurMember >= room.MaxMember)
            {
				S_RejectEnter rejectPacket = new S_RejectEnter();
				rejectPacket.Reason = RejectionReason.FullRoom;
				user.Session.Send(rejectPacket);
				return;
			}

			if(room.Password == packet.PassWord)
            {
				S_AcceptEnter acceptPacket = new S_AcceptEnter();
				user.Session.Send(acceptPacket);
				user.Room = 

                Console.WriteLine($"{user.Name}님이 {room.Name}에 입장하셨습니다.");
            }
            else
            {
				S_RejectEnter rejectPacket = new S_RejectEnter();
				rejectPacket.Reason = RejectionReason.IncorrectPassword;
				user.Session.Send(rejectPacket);
            }
        }

		public void UpdateRoom(RoomInfo info)
        {
			_rooms[info.RoomId].Info = info;

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
