using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using Google.Protobuf;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class NetworkManager
{
	ServerSession _session = new ServerSession();
	ServerSession _hostSession = new ServerSession();
	public bool _host = false;

	public void S_Send(IMessage packet)
	{
		_session.Send(packet);
	}

	public void H_Send(IMessage packet)
	{
        if (_hostSession.CheckSocket())
			_hostSession.Send(packet);
	}

	public void Init(int port = 7777, string ip = null)
	{
		IPAddress ipAddr;
		if (ip == null)
		{
            //string host = "DDuKi.iptime.org";
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
			ipAddr = IPAddress.Parse($"{ipHost.AddressList[1].ToString()}");
		}
		else
			ipAddr = IPAddress.Parse(ip);

		IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

		Connector connector = new Connector();

		connector.Connect(endPoint,
			() => { return _session; },
			1);

		//GameObject go = GameObject.Find("Host");
		//go = Managers.Resource.Instantiate("Host/Host");
		//UnityEngine.Object.DontDestroyOnLoad(go);
	}

	public void Clear()
    {
        _hostSession = new ServerSession();
    }

    public async Task ConnectAsync(string publicIp, string privateIp, int port = 7778, int timeout = 5000)
	{
		AsyncConnector _connector = new AsyncConnector();
		CancellationTokenSource cts = new CancellationTokenSource();
		cts.CancelAfter(timeout);

		try
		{
			IPAddress ipAddr = IPAddress.Parse(privateIp);
			IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

			bool isConnect = await _connector.ConnectAsync(endPoint, () => { isConnect = true; return _hostSession; });

			if (isConnect == false)
			{
				ipAddr = IPAddress.Parse(publicIp);
				endPoint = new IPEndPoint(ipAddr, port);

				isConnect = await _connector.ConnectAsync(endPoint, () => { isConnect = true; return _hostSession; });
			}
		}
		catch (OperationCanceledException)
		{
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

	public void Update()
	{
		List<PacketMessage> list = PacketQueue.Instance.PopAll();
		foreach (PacketMessage packet in list)
		{
			Action<PacketSession, IMessage> handler = Client.PacketManager.Instance.GetPacketHandler(packet.Id);
			if (handler != null)
				handler.Invoke(_session, packet.Message);
		}
	}
}

//IPAddress ipAddr = ipHost.AddressList[0];
//IPAddress ipAddr = IPAddress.Parse("192.168.0.6");
//IPAddress ipAddr = IPAddress.Parse("192.168.219.106");