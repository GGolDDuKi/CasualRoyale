﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using ServerCore;

namespace Server
{
	class Program
	{
		static Listener _listener = new Listener();
		static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();

		static void TickLobby(Lobby lobby, int tick = 1000)
		{
			var timer = new System.Timers.Timer();
			timer.Interval = tick;
			timer.Elapsed += ((s, e) => { lobby.Update(); });
			timer.AutoReset = true;
			timer.Enabled = true;

			_timers.Add(timer);
		}

		static void Main(string[] args)
		{
			Lobby lobby = LobbyManager.Instance.Add();
			TickLobby(lobby, 50);

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
			}
		}
	}
}
//IPAddress ipAddr = ipHost.AddressList[0];
//IPAddress ipAddr = IPAddress.Parse("192.168.0.6");
//IPAddress ipAddr = IPAddress.Parse("192.168.219.106");
//IPAddress ipAddr = IPAddress.Parse($"{ipHost.AddressList[1].ToString()}");