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
		room = HostServer.Game.RoomManager.Instance.Add();
		TickRoom(room, 50);

        // DNS (Domain Name System)

        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        //IPAddress ipAddr = IPAddress.Parse($"{ipHost.AddressList[1].ToString()}");
        IPAddress ipAddr = IPAddress.Parse("0.0.0.0");
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7778);

		_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
		Debug.Log("Listening...");
	}

    public void Clear()
    {
        //SessionManager.Instance.Clear();
        _listener.CloseSocket();
		this.transform.parent = Managers.Scene.CurrentScene.transform;
		this.transform.SetParent(null);
		Destroy(this.gameObject);
    }

    public async Task ConnectAsync(string publicIp, string privateIp, int port = 7778, int timeout = 5000)
	{
		CancellationTokenSource cts = new CancellationTokenSource();
		cts.CancelAfter(timeout);

		try
		{
			IPAddress ipAddr = IPAddress.Parse(privateIp);
			IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

			bool isConnect = await _connector.ConnectAsync(endPoint, () => { isConnect = true; return SessionManager.Instance.Generate(); });

			if (isConnect == false)
			{
				ipAddr = IPAddress.Parse(publicIp);
				endPoint = new IPEndPoint(ipAddr, port);

				isConnect = await _connector.ConnectAsync(endPoint, () => { isConnect = true; return SessionManager.Instance.Generate(); });
			}
		}
		catch (OperationCanceledException)
		{
			//일정시간 연결 안되면 예외
			throw new TimeoutException("The operation has timed out.");
		}
	}

	public void Connect(string publicIp, string privateIp, int port = 7778, int timeout = 5000)
	{
		Task.Run(async () =>
		{
			await ConnectAsync(publicIp, privateIp, port, timeout);
		});
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

    private void Update()
    {
   //     foreach(BulletController b in Managers.Object.Bullets.Values)
   //     {
			//if(b.target != null)
   //         {
			//	Player targetPlayer = room.Players[b.target.GetComponent<CreatureController>().Id];
			//	Bullet bullet = room.Projectiles[b.Id] as Bullet;

			//	foreach (Player p in room.Players.Values)
			//	{
			//		if (p.Id == targetPlayer.Id && p != bullet.Owner)
			//		{
   //                     HostServer.Game.GameObject target = p;
			//			target.OnDamaged(bullet, bullet.Owner.StatInfo.Damage);
			//			Debug.Log($"{bullet.Owner.Id}가 {p.Id}를 공격!");
			//			room.Push(room.LeaveGame, bullet.Id);
			//		}
			//	}
			//}
   //     }
    }
}

//IPAddress ipAddr = ipHost.AddressList[0];
//IPAddress ipAddr = IPAddress.Parse("192.168.0.6");
//IPAddress ipAddr = IPAddress.Parse("192.168.219.106");