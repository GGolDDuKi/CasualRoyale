using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace HostServer
{
	class SessionManager
	{
		static SessionManager _session = new SessionManager();
		public static SessionManager Instance { get { return _session; } }

		int _sessionId = 0;
		Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
		object _lock = new object();

		public ClientSession Generate()
		{
			lock (_lock)
			{
				int sessionId = ++_sessionId;

				ClientSession session = new ClientSession();
				session.SessionId = sessionId;
				_sessions.Add(sessionId, session);

				Console.WriteLine($"Connected : {sessionId}");

				return session;
			}
		}

		public ClientSession Generate(ClientSession newSession)
		{
			lock (_lock)
			{
				int sessionId = ++_sessionId;

				ClientSession session = newSession;
				session.SessionId = sessionId;
				_sessions.Add(sessionId, session);

				Console.WriteLine($"Connected : {sessionId}");

				return session;
			}
		}

		public ClientSession Find(int id)
		{
			lock (_lock)
			{
				ClientSession session = null;
				_sessions.TryGetValue(id, out session);
				return session;
			}
		}

		public void Remove(ClientSession session)
		{
			lock (_lock)
			{
				_sessions.Remove(session.SessionId);
			}
		}

		public void DisconnectSession(int sessionId)
        {
			lock(_lock)
            {
				Dictionary<int, ClientSession> sessions = new Dictionary<int, ClientSession>(_sessions);
				ClientSession session;
				if (sessions.TryGetValue(sessionId, out session))
                {
					HC_HostDisconnect packet = new HC_HostDisconnect();
					session.Send(packet);
					session.Disconnect();
                }
			}
		}

		public void Clear()
        {
            lock (_lock)
            {
				Dictionary<int, ClientSession> sessions = new Dictionary<int, ClientSession>(_sessions);
				HC_HostDisconnect disconnectPacket = new HC_HostDisconnect();
				foreach (ClientSession session in sessions.Values)
				{
					if (session.MyPlayer.Authority != Authority.Host)
						session.Send(disconnectPacket);
					session.Disconnect();
				}
				sessions.Clear();
			}
        }
	}
}
