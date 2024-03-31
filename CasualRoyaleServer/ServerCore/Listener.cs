using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
	public class Listener
	{
		Socket _listenSocket;
		Func<Session> _sessionFactory;

		public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
		{
			_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_sessionFactory += sessionFactory;

			//소켓 준비 작업
			_listenSocket.Bind(endPoint);

			// 영업 시작
			// backlog : 최대 대기수
			_listenSocket.Listen(backlog);

			for (int i = 0; i < register; i++)
			{
				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
				RegisterAccept(args);
			}
		}

		//소켓의 연결 시도를 등록
		void RegisterAccept(SocketAsyncEventArgs args)
		{
			args.AcceptSocket = null;

			bool pending = _listenSocket.AcceptAsync(args);
			if (pending == false)
				OnAcceptCompleted(null, args);
		}

		//소켓의 연결을 처리 후 초기화
		void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				Session session = _sessionFactory.Invoke();
				session.Start(args.AcceptSocket);
				session.OnConnected(args.AcceptSocket.RemoteEndPoint);
			}
			else
				Console.WriteLine(args.SocketError.ToString());

			RegisterAccept(args);
		}
	}
}
