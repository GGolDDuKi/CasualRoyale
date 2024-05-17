using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace ServerCore
{
	public class AsyncConnector
	{
		Func<Session> _sessionFactory;

		public async Task<bool> ConnectAsync(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
		{
			bool isConnected = false;
			for (int i = 0; i < count; i++)
			{
				Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

				try
				{
					IPAddress ipAddr = IPAddress.Parse($"{Managers.User.PrivateIp}");
					IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 50000);
					socket.Bind(localEndPoint);
				}
				catch (SocketException ex)
				{
					if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
					{
						Debug.Log("이미 바인딩된 엔드포인트입니다.");
					}
				}

				_sessionFactory = sessionFactory;

				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += OnConnectCompleted;
				args.RemoteEndPoint = endPoint;
				args.UserToken = socket;

				isConnected = await RegisterConnectAsync(args);
				if (isConnected)
					break;
			}
			return isConnected;
		}

		async Task<bool> RegisterConnectAsync(SocketAsyncEventArgs args)
		{
			Socket socket = args.UserToken as Socket;
			if (socket == null)
				return false;

			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			args.Completed += (sender, e) => { tcs.SetResult(e.SocketError == SocketError.Success); };
			bool pending = socket.ConnectAsync(args);

			if (pending == false)
				tcs.SetResult(args.SocketError == SocketError.Success);

			return await tcs.Task;
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
				//Debug.Log($"OnConnectCompleted Fail: {args.SocketError}");
			}
		}
	}
}
