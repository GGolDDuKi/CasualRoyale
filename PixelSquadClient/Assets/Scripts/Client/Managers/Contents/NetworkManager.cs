using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using Google.Protobuf;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System.Net.NetworkInformation;
using TMPro;

public class NetworkManager
{
	ServerSession _session = new ServerSession();
	ServerSession _hostSession = new ServerSession();
	public bool _host = false;
    public int _port = default(int);

	public void S_Send(IMessage packet)
	{
		_session.Send(packet);
	}

	public void H_Send(IMessage packet)
	{
		_hostSession.Send(packet);
	}

	public void Init(int port = 7777, string ip = null)
	{
		IPAddress ipAddr;
		if (ip == null)
		{
            string host = "DDuKi.iptime.org";
            //string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress[] ipAddrs = ipHost.AddressList;
            ipAddr = IPAddress.Parse($"{ipAddrs[ipAddrs.Length - 1].ToString()}");
		}
		else
			ipAddr = IPAddress.Parse(ip);

		IPEndPoint endPoint = new IPEndPoint(ipAddr, port);
		Connector connector = new Connector();
        GameObject.Find("ServerEndPoint").GetComponent<TMP_Text>().text = $"{endPoint}";

        GetPortNumber();
        connector.Connect(endPoint, () => { return _session; }, true, _port, 1);
	}

    public void GetPortNumber()
    {
        _port = UnityEngine.Random.Range(49152, 65535);
        Debug.Log($"{_port}포트 할당");
    }

    public void Clear()
    {
        _hostSession = new ServerSession();
    }

    public void Connect(string publicIp, string privateIp, int port = 50000, bool bind = false, int count = 3)
    {
        IPAddress ipAddr = IPAddress.Parse(privateIp);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 50000);

        Connector _connector = new Connector();
        _connector.Connect(endPoint, () => { return _hostSession; }, bind, _port, count);

        ipAddr = IPAddress.Parse(publicIp);
        endPoint = new IPEndPoint(ipAddr, port);
        _connector.Connect(endPoint, () => { return _hostSession; }, bind, _port, count);

        //TODO : 연결 안 된 경우 처리
    }

    public void ConnectSelf(string ip, int port = 20000)
    {
        IPAddress ipAddr = IPAddress.Parse(ip);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 20000);

        Connector _connector = new Connector();
        _connector.Connect(endPoint, () => { return _hostSession; }, false, 20000, 1);

        //TODO : 연결 안 된 경우 처리
    }

    //public async Task ConnectAsync(string publicIp, string privateIp, int port = 50000, int timeout = 5000)
    //{
    //    AsyncConnector _connector = new AsyncConnector();
    //    CancellationTokenSource cts = new CancellationTokenSource();
    //    cts.CancelAfter(timeout);

    //    try
    //    {
    //        IPAddress ipAddr = IPAddress.Parse(privateIp);
    //        IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

    //        bool isConnect = await _connector.ConnectAsync(endPoint, () => { isConnect = true; return _hostSession; });

    //        if (isConnect == false)
    //        {
    //            ipAddr = IPAddress.Parse(publicIp);
    //            endPoint = new IPEndPoint(ipAddr, port);

    //            isConnect = await _connector.ConnectAsync(endPoint, () => { isConnect = true; return _hostSession; });
    //        }
    //    }
    //    catch (OperationCanceledException)
    //    {
    //        throw new TimeoutException("The operation has timed out.");
    //    }
    //}

    //public void Connect(string publicIp, string privateIp, int port = 50000, int timeout = 5000)
    //{
    //    Task.Run(async () =>
    //    {
    //        await ConnectAsync(publicIp, privateIp, port, timeout);
    //    });
    //}

    public void StartListen()
    {
        IPAddress ipAddr = IPAddress.Parse("0.0.0.0");
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 50000);
        Listener _listener = new Listener();
        _listener.Init(endPoint, () => { return _hostSession; });
        Debug.Log("Listening...");

        // 5초 후에 Listen을 중지합니다.
        Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(t => StopListen(_listener));
    }

    public void StopListen(Listener listener)
	{
		if (listener != null)
		{
			listener.CloseSocket();
			Debug.Log("Stopped listening...");
		}
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