using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	public class LobbyManager
	{
		public static LobbyManager Instance { get; } = new LobbyManager();

		object _lock = new object();
		Dictionary<int, Lobby> _lobby = new Dictionary<int, Lobby>();
		int _lobbyId = 1;

		public Lobby Add()
		{
			Lobby lobby = new Lobby();

			lock (_lock)
			{
				lobby.LobbyId = _lobbyId;
				_lobby.Add(_lobbyId, lobby);
				_lobbyId++;
			}

			return lobby;
		}

		public bool Remove(int lobbyId)
		{
			lock (_lock)
			{
				return _lobby.Remove(lobbyId);
			}
		}

		public Lobby Find(int lobbyId)
		{
			lock (_lock)
			{
				Lobby lobby = null;
				if (_lobby.TryGetValue(lobbyId, out lobby))
					return lobby;

				return null;
			}
		}
	}
}
