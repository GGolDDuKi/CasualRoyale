using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HostServer;
using HostServer.Game;
using ServerCore;
using UnityEngine;

class HostPlayer : MonoBehaviour
{
	private Listener _listener = new Listener();
	private AsyncConnector _connector = new AsyncConnector();
	private List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();
	public GameRoom room;

	void Awake()
    {
		MakeRoom();
		StartListen();
    }

	public void StartListen()
	{
		IPAddress ipAddr = IPAddress.Parse("0.0.0.0");
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 20000);
		Listener _listener = new Listener();
		_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); }, 1);
		Debug.Log("Listening...");

		// 3초 후에 Listen을 중지합니다.
		Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(t => StopListen(_listener));
	}

	public void StopListen(Listener listener)
	{
		if (listener != null)
		{
			listener.CloseSocket();
			Debug.Log("Stopped listening...");
		}
	}

	void MakeRoom()
    {
		room = HostServer.Game.RoomManager.Instance.Add();
		TickRoom(room, 50);
	}

	void RemoveRoom()
    {
		room = null;

		foreach (var timer in _timers)
		{
			timer.Stop();
			timer.Dispose();
		}
		_timers.Clear();
	}

    public bool Clear()
    {
        _listener.CloseSocket();
        SessionManager.Instance.Clear();
		RemoveRoom();
        this.transform.parent = Managers.Scene.CurrentScene.transform;
        this.transform.SetParent(null);
        Destroy(this.gameObject);
		return true;
    }

	void TickRoom(GameRoom room, int tick = 100)
	{
		var timer = new System.Timers.Timer();
		timer.Interval = tick;
		timer.Elapsed += ((s, e) => { room.Update(); });
		timer.AutoReset = true;
		timer.Enabled = true;

		_timers.Add(timer);
	}
}

//IPAddress ipAddr = ipHost.AddressList[0];
//IPAddress ipAddr = IPAddress.Parse("192.168.0.6");
//IPAddress ipAddr = IPAddress.Parse("192.168.219.106");