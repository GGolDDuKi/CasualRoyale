using Google.Protobuf.Protocol;
using Server;
using Server.Room;
using System;
using System.Collections.Generic;
using System.Text;

public class User
{
	public ClientSession Session { get; set; }
	public Lobby Lobby { get; set; }
	public GameRoom Room { get; set; }

	#region UserInfo Info

	public UserInfo Info { get; set; } = new UserInfo();

	public int Id
	{
		get { return Info.Id; }
		set { Info.Id = value; }
	}

	public string Name
	{
		get { return Info.Name; }
		set { Info.Name = value; }
	}

	public JobType JobType
    {
		get { return Info.Job; }
		set { Info.Job = value; }
    }

	#endregion
}