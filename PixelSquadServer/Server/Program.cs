using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Server.Data;
using Server.Job;
using ServerCore;

namespace Server
{
	class Program
	{
		static Listener _listener = new Listener();
		static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();
        static Dictionary<JobSerializer, System.Timers.Timer> _roomTimers = new Dictionary<JobSerializer, System.Timers.Timer>();
        public static Lobby Lobby = new Lobby();

        public static void TickRoom<T>(T room, int tick = 100) where T : JobSerializer
        {
            var timer = new System.Timers.Timer();
            timer.Interval = tick;
            timer.Elapsed += ((s, e) => { room.Update(); });
            timer.AutoReset = true;
            timer.Enabled = true;

            _roomTimers.Add(room, timer);
        }

        public static void StopTickRoom<T>(T room) where T : JobSerializer
        {
            if (!_roomTimers.TryGetValue(room, out System.Timers.Timer timer))
                return;

            timer.Stop();
            timer.Dispose();
            _roomTimers.Remove(room);
        }

        static void Main(string[] args)
		{
            DataManager.Instance.LoadData();

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
			
			IPAddress ipAddr = IPAddress.Parse("0.0.0.0");
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
            
			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });

			Console.WriteLine("Listening...");

			// TODO
			while (true)
			{
				Thread.Sleep(100);
                Lobby.Update();
            }
		}
	}
}