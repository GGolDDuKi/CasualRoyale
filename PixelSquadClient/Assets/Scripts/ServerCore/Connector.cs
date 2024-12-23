using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using TMPro;

namespace ServerCore
{
	public class Connector
	{
		Func<Session> _sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, bool bind = false, int port = 50000, int count = 1)
        {
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            if (bind)
            {
                try
                {
                    IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
                    Debug.Log($"{localEndPoint} 바인딩 시도");
                    socket.Bind(localEndPoint);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    {
                        Debug.Log("이미 바인딩된 엔드포인트입니다.");
                        Managers.Network.GetPortNumber();
                        Connect(endPoint, sessionFactory, bind, Managers.Network._port, count);
                        return;
                    }
                    else
                    {
                        GameObject.Find("LocalEndPoint").GetComponent<TMP_Text>().text = $"{ex}";
                    }
                }
            }
            Debug.Log($"{port}포트 바인딩 성공");

            for (int i = 0; i < count; i++)
            {
                _sessionFactory = sessionFactory;

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;
                args.UserToken = socket;

                RegisterConnect(args);
            }
        }


        void RegisterConnect(SocketAsyncEventArgs args)
		{
			Socket socket = args.UserToken as Socket;
			if (socket == null)
				return;

			bool pending = socket.ConnectAsync(args);
			if (pending == false)
				OnConnectCompleted(null, args);
		}

		void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				Session session = _sessionFactory.Invoke();
				session.Start(args.ConnectSocket);
				session.OnConnected(args.RemoteEndPoint);
			}
			else
			{
				Debug.Log($"OnConnectCompleted Fail: {args.SocketError}");
				args.Dispose();
			}
		}
	}
}
