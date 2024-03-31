﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HostServer.Game
{
	public class RoomManager
	{
		public static RoomManager Instance { get; } = new RoomManager();

		object _lock = new object();
		Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
		int _roomId = 1;

		public GameRoom Add()
		{
			GameRoom gameRoom = new GameRoom();
			gameRoom.Push(gameRoom.Init);

			lock (_lock)
			{
				gameRoom.RoomId = _roomId;
				_rooms.Add(_roomId, gameRoom);
				Console.WriteLine($"Room[{gameRoom.RoomId}] 생성");
				_roomId++;
			}

			return gameRoom;
		}

		public bool Remove(int roomId)
		{
			lock (_lock)
			{
				return _rooms.Remove(roomId);
			}
		}

		public GameRoom Find(int roomId)
		{
			lock (_lock)
			{
				GameRoom room = null;
				if (_rooms.TryGetValue(roomId, out room))
					return room;

				return null;
			}
		}
	}
}
