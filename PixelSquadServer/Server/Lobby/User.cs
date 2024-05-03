using Google.Protobuf.Protocol;
using Server;
using System;
using System.Collections.Generic;
using System.Text;

public class User
{
	public ClientSession Session { get; set; }
	public Lobby Lobby { get; set; }

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

	public string PublicIp
    {
		get { return Info.PublicIp; }
		set { Info.PublicIp = value; }
    }

	public string PrivateIp
	{
		get { return Info.PrivateIp; }
		set { Info.PrivateIp = value; }
	}

	public int Port
    {
		get { return Info.Port; }
		set { Info.Port = value; }
    }

	public JobType JobType
    {
		get { return Info.Job; }
		set { Info.Job = value; }
    }

	#endregion
}